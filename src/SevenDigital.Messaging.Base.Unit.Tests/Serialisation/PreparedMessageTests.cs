using System;
using System.Text;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;

namespace Messaging.Base.Unit.Tests.Serialisation
{
	[TestFixture]
	public class PreparedMessageTests
	{
		const string typeString = "this.is.my.type";
		const string messageString = "{'a' : '|b'}";
		private const string routingKey = "myRoutingKey";
		private const string serialisedOldFormat = "this.is.my.type|{'a' : '|b'}";
		private const string serialisedNewFormat = "V2=this.is.my.type|myRoutingKey|{'a' : '|b'}";

		[Test]
		public void should_be_able_to_round_trip_between_bytes_and_strings()
		{
			var originalMessage = new PreparedMessage(typeString, messageString, routingKey);
			var bytes = originalMessage.ToBytes();
			var result = PreparedMessage.FromBytes(bytes);
			Assert.That(result.TypeName(), Is.EqualTo(typeString));
			Assert.That(result.RoutingKey, Is.EqualTo(routingKey));
			Assert.That(result.SerialisedMessage(), Is.EqualTo(messageString));
		}

		[Test]
		public void should_be_able_to_read_old_format_messages()
		{
			var bytes = Encoding.UTF8.GetBytes(serialisedOldFormat);
			var result = PreparedMessage.FromBytes(bytes);

			Assert.That(result.TypeName(), Is.EqualTo(typeString));
			Assert.That(result.RoutingKey, Is.EqualTo(String.Empty));
			Assert.That(result.SerialisedMessage(), Is.EqualTo(messageString));
		}

		[Test]
		public void should_be_able_to_read_new_routing_key_messages()
		{
			var bytes = Encoding.UTF8.GetBytes(serialisedNewFormat);
			var result = PreparedMessage.FromBytes(bytes);

			Assert.That(result.TypeName(), Is.EqualTo(typeString));
			Assert.That(result.RoutingKey, Is.EqualTo(routingKey));
			Assert.That(result.SerialisedMessage(), Is.EqualTo(messageString));
		}

	}
}