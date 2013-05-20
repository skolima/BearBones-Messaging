using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public class RabbitMqConnection : IRabbitMqConnection
	{
		public string Host { get; private set; }
		public string VirtualHost { get; private set; }

        public RabbitMqConnection(string hostUri, string virtualHost = "/")
        {
	        Host = hostUri;
	        VirtualHost = virtualHost;
        }

		public ConnectionFactory ConnectionFactory()
        {
            return new ConnectionFactory
                {
                    Protocol = Protocols.FromEnvironment(),
                    HostName = Host,
                    VirtualHost = VirtualHost,
                };
        }
		
		public void WithChannel(Action<IModel> actions)
		{
			var factory = ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				actions(channel);
				if (channel.IsOpen) channel.Close();
				if (conn.IsOpen) conn.Close();
			}
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			var factory = ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				var result = actions(channel);
				if (channel.IsOpen) channel.Close();
				if (conn.IsOpen) conn.Close();
				return result;
			}
		}

		public void Dispose()
		{
		}
	}
}
