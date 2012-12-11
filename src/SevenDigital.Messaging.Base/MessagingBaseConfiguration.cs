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
			ObjectFactory.Configure(map => {
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
			ObjectFactory.Configure(map => map.For<IRabbitMqConnection>().Use(()=> configuredConnection));
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
		
		public MessagingBaseConfiguration WithRabbitManagement(string host, string username, string password, string vhost)
		{
			ObjectFactory.Configure(map => map.For<IRabbitMqQuery>().Use(() => new RabbitMqQuery("http://" + host + ":55672", username, password, vhost) ));
			return this;
		}
		
        static RabbitMqQuery RabbitMqQueryWithConfigSettings()
        {
            var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
            var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
            var username = ConfigurationManager.AppSettings["ApiUsername"];
            var password = ConfigurationManager.AppSettings["ApiPassword"];
            var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

            //return new RabbitMqManagement("http://" + hostUri + ":15672", username, password, vhost); // 3.0+
            return new RabbitMqQuery("http://" + hostUri + ":55672", username, password, vhost); // before RMQ 3
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
