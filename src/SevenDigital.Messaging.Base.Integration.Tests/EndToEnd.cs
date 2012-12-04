using System;
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

		[SetUp]
		public void A_configured_messaging_base ()
		{
			new MessagingBaseConfiguration()
				.WithDefaults()
				.WithConnectionFromAppConfig();

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
			MessagingBase.CreateDestination<IMsg>("Test_Destination");
			MessagingBase.SendMessage(testMessage);

			var finalObject = (IMetadataFile)MessagingBase.GetMessage<IMsg>("Test_Destination");

			Assert.That(finalObject, Is.Not.Null);
			Assert.That(finalObject.CorrelationId, Is.EqualTo(testMessage.CorrelationId));
			Assert.That(finalObject.Contents, Is.EqualTo(testMessage.Contents));
			Assert.That(finalObject.FilePath, Is.EqualTo(testMessage.FilePath));
			Assert.That(finalObject.HashValue, Is.EqualTo(testMessage.HashValue));
			Assert.That(finalObject.MetadataName, Is.EqualTo(testMessage.MetadataName));
			Assert.That(finalObject.Equals(testMessage), Is.False);
		}

		[Test, Explicit]
		public void Should_be_able_to_send_and_receive_1000_messages_in_a_minute ()
		{
			MessagingBase.CreateDestination<IMsg>("Test_Destination");

			int sent = 1000;
			int received = 0;
			var start = DateTime.Now;
			for (int i = 0; i < sent; i++)
			{
				MessagingBase.SendMessage(testMessage);
			}

			Console.WriteLine("Sending took "+((DateTime.Now) - start));
			var startGet = DateTime.Now;

			while (MessagingBase.GetMessage<IMsg>("Test_Destination") != null)
			{
				received++;
			}
			Console.WriteLine("Receiving took "+((DateTime.Now) - startGet));

			var time = (DateTime.Now) - start;
			Assert.That(received, Is.EqualTo(sent));
			Assert.That(time.TotalSeconds, Is.LessThanOrEqualTo(60));
		}

		[TearDown]
		public void cleanup ()
		{
			((RabbitRouter)ObjectFactory.GetInstance<IMessageRouter>()).RemoveRouting();
		}
	}
}
