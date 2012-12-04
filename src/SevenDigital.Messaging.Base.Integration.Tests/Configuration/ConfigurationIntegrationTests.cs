using System;
using System.Configuration;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;
using StructureMap;

namespace Messaging.Base.Unit.Tests.Configuration
{
	[TestFixture]
    public class ConfigurationIntegrationTests
    {
		IRabbitMqConnection connection;
		IRabbitMqQuery query;

		[SetUp]
		public void When_configuring_the_messaging_base_with_app_config_settings ()
		{
			new MessagingBaseConfiguration().WithConnectionFromAppConfig();
			connection = ObjectFactory.GetInstance<IRabbitMqConnection>();
			query = ObjectFactory.GetInstance<IRabbitMqQuery>();
		}

		[Test]
		public void Should_have_rabbit_mq_connection_in_configuration()
		{
			Assert.That(connection, Is.InstanceOf<RabbitMqConnection>());
		}

		[Test]
		public void Rabbit_mq_connection_should_have_host_uri_from_app_config ()
		{
			var host = ConfigurationManager.AppSettings["Messaging.Host"].SubstringBefore('/');
			Assert.That(connection.Host, Is.EqualTo(host));
		}
		
		[Test]
		public void Rabbit_mq_connection_should_have_virtual_host_from_app_config ()
		{
			var vhost = ConfigurationManager.AppSettings["Messaging.Host"].SubstringAfter('/');
			if (string.IsNullOrEmpty(vhost)) vhost = "/";

			Assert.That(connection.Host, Is.EqualTo(vhost));
		}

		[Test]
		public void Should_have_RabbitMq_management_api_query_in_configuration ()
		{
			Assert.That(query, Is.InstanceOf<RabbitMqQuery>());
		}
    }
}
