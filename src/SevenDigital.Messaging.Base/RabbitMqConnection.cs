using System;
using System.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace SevenDigital.Messaging.Base
{
	public class RabbitMqConnection : IRabbitMqConnection
	{
		readonly string hostUri;
		readonly string virtualHost;

        public RabbitMqConnection(string hostUri, string virtualHost = "/")
        {
	        this.hostUri = hostUri;
	        this.virtualHost = virtualHost;
        }

		public static RabbitMqConnection WithAppConfigSettings()
		{
            var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
            var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
            var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

            return new RabbitMqConnection(hostUri, vhost);
		}

		public ConnectionFactory ConnectionFactory()
        {
            return new ConnectionFactory
                {
                    Protocol = Protocols.FromEnvironment(),
                    HostName = hostUri,
                    VirtualHost = virtualHost,
                };
        }
		
		public void WithChannel(Action<IModel> actions)
		{
			var factory = ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				actions(channel);
				channel.Close();
				conn.Close();
			}
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			var factory = ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				var result = actions(channel);
				channel.Close();
				conn.Close();
				return result;
			}
		}
		
		public IBasicProperties EmptyBasicProperties()
		{
			return new BasicProperties();
		}
	}
}
