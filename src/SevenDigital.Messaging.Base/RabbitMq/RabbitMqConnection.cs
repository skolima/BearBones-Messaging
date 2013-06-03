using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	/// <summary>
	/// Default short-term connection.
	/// This class opens and closes a connection per request, and
	/// should not be used for polling.
	/// </summary>
	public class RabbitMqConnection : IRabbitMqConnection
	{
		/// <summary>
		/// Rabbit MQ Cluster host name uri fragment
		/// </summary>
		public string Host { get; private set; }

		/// <summary>
		/// Target virtual host
		/// </summary>
		public string VirtualHost { get; private set; }

		/// <summary>
		/// Prepare a connection provider
		/// </summary>
		public RabbitMqConnection(string hostUri, string virtualHost = "/")
		{
			Host = hostUri;
			VirtualHost = virtualHost;
		}

		/// <summary>
		/// Return a connection factory.
		/// Use this to connect to the RMQ cluster.
		/// ALWAYS dispose your connections.
		/// </summary>
		public ConnectionFactory ConnectionFactory()
		{
			return new ConnectionFactory
				{
					Protocol = Protocols.FromEnvironment(),
					HostName = Host,
					VirtualHost = VirtualHost,
				};
		}

		/// <summary>
		/// Perform an action against the RMQ cluster, returning no data
		/// </summary>
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

		/// <summary>
		/// Perform an action against the RMQ cluster, returning data
		/// </summary>
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

		/// <summary>
		/// No action.
		/// </summary>
		public void Dispose()
		{
		}
	}
}
