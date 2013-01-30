using System;
using System.Threading;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using StructureMap;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class EndToEnd
	{
		SuperMetadata testMessage;
		IMessagingBase messaging;

		[TestFixtureSetUp]
		public void A_configured_messaging_base ()
		{
			new MessagingBaseConfiguration()
				.WithDefaults()
				.WithConnectionFromAppConfig();

			messaging = ObjectFactory.GetInstance<IMessagingBase>();

			testMessage = new SuperMetadata{
				CorrelationId = Guid.NewGuid(),
				Contents = "These are my contents: ⰊⰄⰷἚ𐰕𐰑ꔘⶤعبػػ↴↳↲↰",
				FilePath = @"C:\temp\",
				HashValue = 893476,
				MetadataName = "KeyValuePair"
			};
		}

		[Test]
		public void Should_be_able_to_send_and_receive_messages_by_interface_type_and_destination_name ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination");
			messaging.SendMessage(testMessage);

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
		public void Should_be_able_to_send_and_receive_messages_by_destination_name_and_get_correct_type ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination");
			messaging.SendMessage(testMessage);

			var finalObject = (IMetadataFile)messaging.GetMessage("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test]
		public void Should_be_able_to_send_and_receive_1000_messages_in_a_minute ()
		{
			messaging.CreateDestination<IMsg>("Test_Destination");

			int sent = 1000;
			int received = 0;
			var start = DateTime.Now;
			for (int i = 0; i < sent; i++)
			{
				messaging.SendMessage(testMessage);
			}

			Console.WriteLine("Sending took "+((DateTime.Now) - start));
			var startGet = DateTime.Now;

			while (messaging.GetMessage<IMsg>("Test_Destination") != null)
			{
				Interlocked.Increment(ref received);
			}
			Console.WriteLine("Receiving took "+((DateTime.Now) - startGet));

			var time = (DateTime.Now) - start;
			Assert.That(received, Is.EqualTo(sent));
			Assert.That(time.TotalSeconds, Is.LessThanOrEqualTo(60));
		}

		[TestFixtureTearDown]
		public void cleanup ()
		{
			((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting(n=>true);
		}
	}
}
