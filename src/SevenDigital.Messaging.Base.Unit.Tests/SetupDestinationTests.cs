using System;
using Example.Types;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;

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

			messaging = new MessagingBase(typeRouter, messageRouter, serialiser);
			messaging.ResetCaches();
			messaging.CreateDestination<IMetadataFile>("MyServiceDestination", String.Empty, ExchangeType.Direct);
		}

		[Test]
		public void Should_setup_type_routing_for_listening_type ()
		{
			typeRouter.Received().BuildRoutes(typeof(IMetadataFile), String.Empty, ExchangeType.Direct);
		}

		[Test]
		public void Should_setup_destination ()
		{
			messageRouter.Received().AddDestination("MyServiceDestination");
		}

		[Test]
		public void Should_link_destination_to_target_type_source ()
		{
			messageRouter.Received().Link("Example.Types.IMetadataFile", "MyServiceDestination", String.Empty);
		}
	}
}
