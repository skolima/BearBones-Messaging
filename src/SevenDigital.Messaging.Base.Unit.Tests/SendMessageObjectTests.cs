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
		SuperMetadata metadataMessage;
		object badMessage;
		string result;
		IMessagingBase messaging;
		private const string serialisedObject = "serialised object";

		[SetUp]
		public void When_setting_up_a_named_destination ()
		{
			metadataMessage = new SuperMetadata();

			badMessage = new {Who="What"};
			typeRouter = Substitute.For<ITypeRouter>();
			messageRouter = Substitute.For<IMessageRouter>();
			serialiser = Substitute.For<IMessageSerialiser>();
			serialiser.Serialise(metadataMessage).Returns(serialisedObject);

			MessagingBase.ResetCaches();
			messaging = new MessagingBase(typeRouter, messageRouter, serialiser);
			result = messaging.SendMessage(metadataMessage);
		}

		[Test]
		public void Should_setup_type_message_type_if_not_already_in_place()
		{
			result = messaging.SendMessage(metadataMessage);
			typeRouter.Received().BuildRoutes(typeof(IMetadataFile));
		}

		[Test]
		public void Should_serialise_the_message()
		{
			serialiser.Received().Serialise(metadataMessage);
		}

		[Test]
		public void Should_throw_exception_when_sending_message_without_exactly_one_parent_interface()
		{
			var ex = Assert.Throws<ArgumentException>(() => messaging.SendMessage(badMessage));
			Assert.That(ex.Message, Contains.Substring("Messages must directly implement exactly one interface"));
		}

		[Test]
		public void Should_send_a_message_to_source()
		{
			var source = typeof (IMetadataFile).FullName;
			messageRouter.Received().Send(source, serialisedObject);
		}

		[Test]
		public void Should_return_serialised_message ()
		{
			Assert.That(result, Is.EqualTo(serialisedObject));
		}
	}

}
