using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface ILongTermConnection:IDisposable,IChannelAction
	{
	}

	public class LongTermRabbitConnection : ILongTermConnection
	{
		readonly IRabbitMqConnection rabbitMqConnection;
		ConnectionFactory factory;
		IConnection conn;
		IModel channel;

		public LongTermRabbitConnection(IRabbitMqConnection rabbitMqConnection)
		{
			this.rabbitMqConnection = rabbitMqConnection;
		}

		~LongTermRabbitConnection()
		{
			Shutdown();
		}

		public void Dispose()
		{
			Shutdown();
		}

		void Shutdown()
		{
			if (channel != null && channel.IsOpen)
			{
				channel.Close();
			}

			if (conn != null && conn.IsOpen)
			{
				conn.Close();
			}

			DisposeChannel();
			DisposeConnection();
		}

		readonly object lockObject = new Object();

		public void WithChannel(Action<IModel> actions)
		{
			lock (lockObject)
			{
				EnsureChannel();
				actions(channel);
			}
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			lock (lockObject)
			{
				EnsureChannel();
				return actions(channel);
			}
		}

		void EnsureChannel()
		{
			lock (lockObject)
			{
				if (factory == null)
				{
					factory = rabbitMqConnection.ConnectionFactory();
				}
				if (channel != null && channel.IsOpen) return;
				if (conn != null && conn.IsOpen)
				{
					DisposeChannel();
					channel = conn.CreateModel();
					return;
				}

				DisposeConnection();

				conn = factory.CreateConnection();
				channel = conn.CreateModel();
			}
		}

// ReSharper disable EmptyGeneralCatchClause
		void DisposeConnection() { try { conn.Dispose(); } catch { } }

		void DisposeChannel() { try { channel.Dispose(); } catch { } }
// ReSharper restore EmptyGeneralCatchClause
	}
}
