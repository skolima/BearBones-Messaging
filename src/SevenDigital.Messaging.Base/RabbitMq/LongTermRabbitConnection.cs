using System;
using System.Threading;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface ILongTermConnection: IChannelAction { }

	public class LongTermRabbitConnection : ILongTermConnection
	{
		readonly IRabbitMqConnection rabbitMqConnection;
		readonly object lockObject;

		ConnectionFactory _factory;
		IConnection _conn;
		IModel _channel;

		public LongTermRabbitConnection(IRabbitMqConnection rabbitMqConnection)
		{
			lockObject = new Object();
			this.rabbitMqConnection = rabbitMqConnection;
		}

		~LongTermRabbitConnection()
		{
			ShutdownConnection();
		}

		public void Dispose()
		{
			ShutdownConnection();
		}

		public void Reset()
		{
			ShutdownConnection();
		}

		public void WithChannel(Action<IModel> actions)
		{
			actions(EnsureChannel());
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			return actions(EnsureChannel());
		}

		void ShutdownConnection()
		{
			lock (lockObject)
			{
				if (_channel != null && _channel.IsOpen)
				{
					_channel.Close();
				}

				if (_conn != null && _conn.IsOpen)
				{
					_conn.Close();
				}

				DisposeChannel();
				DisposeConnection();
				_factory = null;
			}
		}


		IModel EnsureChannel()
		{
			var lchan = _channel;
			if (lchan != null && lchan.IsOpen) return lchan;

			lock (lockObject)
			{
				if (_factory == null)
				{
					_factory = rabbitMqConnection.ConnectionFactory();
				}
				if (_channel != null && _channel.IsOpen) return _channel;
				if (_conn != null && _conn.IsOpen)
				{
					DisposeChannel();
					_channel = _conn.CreateModel();
					return _channel;
				}

				DisposeConnection();

				var lfac = _factory;
				if (lfac == null) throw new Exception("RabbitMq Connection failed to generate a connection factory");
				_conn = lfac.CreateConnection();

				_channel = _conn.CreateModel();
				return _channel;
			}
		}

		void DisposeConnection() { 
			lock (lockObject)
			{
				//try
				//{
					var conn = Interlocked.Exchange(ref _conn, null);
					if (conn != null) conn.Dispose();
				//}
				//catch
				//{
				//	Console.WriteLine("disposal failed");
				//	Ignore();
				//}
			}
		}

		void DisposeChannel() { 
			lock (lockObject)
			{
				//try
				//{
					var chan = Interlocked.Exchange(ref _channel, null);
					if (chan != null) chan.Dispose();
				//}
				//catch
				//{
				//	Console.WriteLine("disposal failed");
				//	Ignore();
				//}
			}
		}

		static void Ignore() { }
	}
}
