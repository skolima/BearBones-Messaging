﻿using Example.Types;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class SetupDestinationTests
	{
		ITypeRouter typeRouter;
		IMessageRouter messageRouter;
		IMessageSerialiser serialiser;
		IMessagingBase messaging;

		[SetUp]
		public void When_setting_up_a_named_destination ()
		{
			typeRouter = Substitute.For<ITypeRouter>();
			messageRouter = Substitute.For<IMessageRouter>();
			serialiser = Substitute.For<IMessageSerialiser>();
			MessagingBase.ResetRouteCache();

			messaging = new MessagingBase(typeRouter, messageRouter, serialiser);
			messaging.CreateDestination<IMetadataFile>("MyServiceDestination");
		}

		[Test]
		public void Should_setup_type_routing_for_listening_type ()
		{
			typeRouter.Received().BuildRoutes(typeof(IMetadataFile));
		}

		[Test]
		public void Should_setup_destination ()
		{
			messageRouter.Received().AddDestination("MyServiceDestination");
		}

		[Test]
		public void Should_link_destination_to_target_type_source ()
		{
			messageRouter.Received().Link("Example.Types.IMetadataFile", "MyServiceDestination");
		}
	}
}
