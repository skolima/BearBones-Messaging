using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace Messaging.Base.Unit.Tests.Configuration
{
	[TestFixture]
	public class ConfigurationTests
	{
		[SetUp]
		public void A_configured_messaging_base ()
		{
			new MessagingBaseConfiguration().WithDefaults().WithConnection(Substitute.For<IRabbitMqConnection>());
		}

		[Test]
		public void Should_have_message_serialiser ()
		{
			Assert.That(
				ObjectFactory.GetInstance<IMessageSerialiser>(),
				Is.InstanceOf<MessageSerialiser>());
		}

		[Test]
		public void Should_have_rabbitmq_message_router ()
		{
			Assert.That(
				ObjectFactory.GetInstance<IMessageRouting>(),
				Is.InstanceOf<RabbitRouting>());
		}

		[Test]
		public void Should_have_type_structure_router ()
		{
			Assert.That(
				ObjectFactory.GetInstance<ITypeStructureRouter>(),
				Is.InstanceOf<TypeStructureRouter>());
		}
	}
}
