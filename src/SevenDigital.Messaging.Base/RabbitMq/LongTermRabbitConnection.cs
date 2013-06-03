using System;
using System.Threading;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface ILongTermConnection : IChannelAction
	{
		void Reset();
	}

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
			lock (lockObject)
			{
				actions(EnsureChannel());
			}
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			lock (lockObject)
			{
				return actions(EnsureChannel());
			}
		}

		void ShutdownConnection()
		{
			_factory = null;
			DisposeChannel();
			DisposeConnection();
		}

		IModel EnsureChannel()
		{
			var lchan = _channel;
			if (lchan != null && lchan.IsOpen) return lchan;

			if (_factory == null)
			{
				_factory = rabbitMqConnection.ConnectionFactory();
			}
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

		void DisposeConnection()
		{
			lock (lockObject)
			{
				var conn = Interlocked.Exchange(ref _conn, null);
				if (conn == null) return;
				if (conn.IsOpen) conn.Close();
				conn.Dispose();
			}
		}

		void DisposeChannel()
		{
			lock (lockObject)
			{
				var chan = Interlocked.Exchange(ref _channel, null);
				if (chan == null) return;
				if (chan.IsOpen) chan.Close();
				chan.Dispose();
			}
		}
	}
}
