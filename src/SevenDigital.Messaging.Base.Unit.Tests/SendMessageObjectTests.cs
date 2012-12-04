using System;
using Example.Types;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class SendMessageObjectTests
	{
		ITypeRouter typeRouter;
		IMessageRouter messageRouter;
		IMessageSerialiser serialiser;
		MetadataMessage metadataMessage;
		private const string serialisedObject = "serialised object";

		[SetUp]
		public void When_setting_up_a_named_destination ()
		{
			metadataMessage = new MetadataMessage();

			typeRouter = Substitute.For<ITypeRouter>();
			messageRouter = Substitute.For<IMessageRouter>();
			serialiser = Substitute.For<IMessageSerialiser>();
			serialiser.Serialise(metadataMessage).Returns(serialisedObject);

			ObjectFactory.Configure(map => {
				map.For<ITypeRouter>().Use(typeRouter);
				map.For<IMessageRouter>().Use(messageRouter);
				map.For<IMessageSerialiser>().Use(serialiser);
			});

			MessagingBase.SendMesssage(metadataMessage);
		}

		[Test, Ignore]
		public void Should_setup_type_routing_for_listening_type()
		{
			typeRouter.Received().BuildRoutes<IMetadataFile>();
		}

		[Test]
		public void Should_serialise_the_message()
		{
			serialiser.Received().Serialise(metadataMessage);
		}

		[Test, Ignore]
		public void Should_throw_exception_when_sending_message_without_exactly_one_parent_interface()
		{
		}

		[Test]
		public void Should_send_a_message_to_source()
		{
			var source = typeof (IMetadataFile).FullName;
			messageRouter.Received().Send(source, serialisedObject);
		}
	}

	public class MetadataMessage: IMetadataFile
	{
		public MetadataMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
		public int HashValue { get; set; }
		public string FilePath { get; set; }
		public string Contents { get; set; }
		public string MetadataName { get; set; }
	}
}
