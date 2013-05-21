using System.Configuration;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace SevenDigital.Messaging.Base
{
	public class MessagingBaseConfiguration
	{
		IRabbitMqConnection configuredConnection;
		IRabbitMqQuery configuredQuery;

		public MessagingBaseConfiguration WithDefaults()
		{
			ObjectFactory.Configure(map =>
			{
				map.For<IMessageSerialiser>().Use<MessageSerialiser>();
				map.For<ITypeRouter>().Use<TypeRouter>();
				map.For<IMessagingBase>().Use<MessagingBase>();

				map.For<IMessageRouter>().Singleton().Use<RabbitRouter>();
				map.For<IChannelAction>().Singleton().Use<LongTermRabbitConnection>();
			});

			return this;
		}

		public MessagingBaseConfiguration WithConnection(IRabbitMqConnection connection)
		{
			configuredConnection = connection;
			ObjectFactory.Configure(map => map.For<IRabbitMqConnection>().Use(() => configuredConnection));
			return this;
		}

		public MessagingBaseConfiguration WithConnectionFromAppConfig()
		{
			configuredConnection = RabbitMqConnectionWithAppConfigSettings();
			ObjectFactory.Configure(map => map.For<IRabbitMqConnection>().Use(() => configuredConnection));
			return this;
		}

		public MessagingBaseConfiguration WithRabbitManagementFromAppConfig()
		{
			configuredQuery = RabbitMqQueryWithConfigSettings();
			ObjectFactory.Configure(map => map.For<IRabbitMqQuery>().Use(() => configuredQuery));
			return this;
		}

		const int port = 55672; // before RMQ 3; 3 redirects, so we use this for compatibility for now.
		//const int port = 15672; // RMQ 3+

		public MessagingBaseConfiguration WithRabbitManagement(string host, string username, string password, string vhost)
		{
			ObjectFactory.Configure(map => map.For<IRabbitMqQuery>().Use(() =>
				new RabbitMqQuery("http://" + host + ":" + port, username, password, vhost)));
			return this;
		}

		static RabbitMqQuery RabbitMqQueryWithConfigSettings()
		{
			var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
			var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
			var username = ConfigurationManager.AppSettings["ApiUsername"];
			var password = ConfigurationManager.AppSettings["ApiPassword"];
			var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

			return new RabbitMqQuery("http://" + hostUri + ":" + port, username, password, vhost);
		}

		static RabbitMqConnection RabbitMqConnectionWithAppConfigSettings()
		{
			var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
			var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
			var vhost = (parts.Length >= 2 && parts[1].Length > 0) ? (parts[1]) : ("/");

			return new RabbitMqConnection(hostUri, vhost);
		}

	}
}
