﻿using System;
using System.Threading;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class EndToEnd
	{
		SuperMetadata testMessage;
		IMessagingBase messaging;

		[SetUp]
		public void A_configured_messaging_base()
		{
			new MessagingBaseConfiguration()
				.WithDefaults()
				.WithConnection(ConfigurationHelpers.RabbitMqConnectionWithConfigSettings());

			messaging = ObjectFactory.GetInstance<IMessagingBase>();

			testMessage = new SuperMetadata
			{
				CorrelationId = Guid.NewGuid(),
				Contents = "These are my ||\"\\' ' contents: ⰊⰄⰷἚ𐰕𐰑ꔘⶤعبػػ↴↳↲↰",
				FilePath = @"C:\temp\",
				HashValue = 893476,
				MetadataName = "KeyValuePair"
			};
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_by_interface_type_and_destination_name()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Direct);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Direct);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_using_prepare_message_intermediates()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Direct);
			byte[] raw = messaging.PrepareForSend(testMessage, String.Empty, ExchangeType.Direct).ToBytes();

			messaging.SendPrepared(PreparedMessage.FromBytes(raw));

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_by_destination_name_and_get_correct_type()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void should_be_able_to_get_cancel_get_again_and_finish_messages()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			var pending_2 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);
			Assert.That(pending_2, Is.Null);

			pending_1.Cancel();
			pending_2 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_2, Is.Not.Null);

			pending_2.Finish();
			var pending_3 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_3, Is.Null);

			var finalObject = (IMetadataFile)pending_2.Message;
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}


		[Test]
		public void should_protect_from_cancelling_the_same_message_twice ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);

			pending_1.Cancel();
			pending_1.Cancel();

			pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);
			pending_1.Finish();

			Assert.Pass();
		}

		[Test]
		public void should_protect_from_finishing_the_same_message_twice ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);

			pending_1.Finish();
			pending_1.Finish();

			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);
			pending_1.Finish();

			Assert.Pass();
		}

		[Test]
		public void should_protect_from_cancelling_then_finishing_a_message ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);

			pending_1.Cancel();
			pending_1.Finish();

			pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);
			pending_1.Finish();

			Assert.Pass();
		}

		[Test]
		public void should_protect_from_finishing_then_cancelling_a_message ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);
			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			var pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);

			pending_1.Finish();
			pending_1.Cancel();

			messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);

			pending_1 = messaging.TryStartMessage<IMsg>("Test_Destination");
			Assert.That(pending_1, Is.Not.Null);
			pending_1.Finish();

			Assert.Pass();
		}

		[Test]
		public void Should_be_able_to_send_and_receive_1000_messages_in_a_minute()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", String.Empty, ExchangeType.Topic);

			int sent = 1000;
			int received = 0;
			var start = DateTime.Now;
			for (int i = 0; i < sent; i++)
			{
				messaging.SendMessage(testMessage, String.Empty, ExchangeType.Topic);
			}

			Console.WriteLine("Sending took " + ((DateTime.Now) - start));
			var startGet = DateTime.Now;

			while (messaging.GetMessage<IMsg>("Test_Destination") != null)
			{
				Interlocked.Increment(ref received);
			}
			Console.WriteLine("Receiving took " + ((DateTime.Now) - startGet));

			var time = (DateTime.Now) - start;
			Assert.That(received, Is.EqualTo(sent));
			Assert.That(time.TotalSeconds, Is.LessThanOrEqualTo(60));
		}

		[Test]
		public void Should_be_able_to_send_with_a_routing_key_and_receive()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", "routingKey", ExchangeType.Direct);
			messaging.SendMessage(testMessage, "routingKey", ExchangeType.Direct);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void Should_be_able_to_send_with_routing_key_and_receive_when_multiple_destinations_defined()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", "routingKey", ExchangeType.Direct);
			messaging.CreateDestination<IMetadataFile>("Test_Destination", String.Empty, ExchangeType.Direct);

			messaging.SendMessage(new UltraMegaData {CorrelationId = Guid.NewGuid()}, "routingKey", ExchangeType.Direct);

			var finalObject = (IUltraMegaData)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.Not.EqualTo(Guid.Empty));
		}

		[Test]
		public void Should_be_able_to_send_and_recieve_using_specific_routing_key()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", "routingKey", ExchangeType.Direct);
			messaging.SendMessage(testMessage, "routingKey", ExchangeType.Direct);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void Should_not_receieve_a_message_with_routing_key_mismatch_in_destination()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", "routingKey", ExchangeType.Direct);
			messaging.SendMessage(testMessage, "routingKey2", ExchangeType.Direct);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Null);
		}

		[Test]
		public void Should_receive_messages_with_any_routing_key_when_topic_set_appropriately()
		{
			messaging.CreateDestination<IMsg>("Test_Destination", "#", ExchangeType.Topic);
			messaging.SendMessage(testMessage, "routingKey2", ExchangeType.Topic);

			var finalObject = (IMetadataFile)messaging.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[TearDown]
		public void cleanup()
		{
			((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting(n => true);
		}
	}
}
