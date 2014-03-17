using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using RabbitMQ.Client;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.Routing;
using ExchangeType = SevenDigital.Messaging.Base.Routing.ExchangeType;

namespace Messaging.Base.Unit.Tests.TypeRouting
{
	[TestFixture]
	public class RabbitRouterTests
	{
		private IMessageRouter _subject;
		private IRabbitMqConnection _shortTermConnection;
		private IModel _model;

		[SetUp]
		public void SetUp()
		{
			_shortTermConnection = Substitute.For<IRabbitMqConnection>();
			_model = Substitute.For<IModel>();

			_shortTermConnection.WithChannel(Arg.Invoke(_model));

			_subject = new RabbitRouter(null, _shortTermConnection);
		}

		[TestCase(ExchangeType.Direct, "direct")]
		[TestCase(ExchangeType.Fanout, "fanout")]
		[TestCase(ExchangeType.Topic, "topic")]
		public void Should_set_exchange_type_to_proper_value(ExchangeType type, string rabbitExchangeType)
		{
			_subject.AddSource(rabbitExchangeType + "testExchange", type);

			_model.Received().ExchangeDeclare(
				Arg.Any<string>(), rabbitExchangeType, Arg.Any<bool>(), Arg.Any<bool>(),Arg.Any<IDictionary<string, object>>());
		}

		[Test]
		public void Shold_throw_when_exchange_type_is_not_an_expected_value()
		{
			var exception = Assert.Throws<ArgumentOutOfRangeException>(
				() => _subject.AddSource(Guid.NewGuid().ToString(), (ExchangeType) 42));

			Assert.That(exception.Message, Is.StringContaining("42"));
		}
	}
}
