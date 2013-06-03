using System;
using System.Threading;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	/// <summary>
	/// Long-term connection to an RMQ cluster.
	/// This provider should be used when polling.
	/// </summary>
	public interface ILongTermConnection : IChannelAction
	{
		/// <summary>
		/// Close any existing connections.
		/// Connections will be re-opened if an action is requested.
		/// </summary>
		void Reset();
	}

	/// <summary>
	/// Default long-term connection to an RMQ cluster.
	/// This provider should be used when polling.
	/// </summary>
	public class LongTermRabbitConnection : ILongTermConnection
	{
		readonly IRabbitMqConnection rabbitMqConnection;
		readonly object lockObject;

		ConnectionFactory _factory;
		IConnection _conn;
		IModel _channel;

		/// <summary>
		/// Prepare a long term connection with a connection provider.
		/// Call `MessagingBaseConfiguration` and request IChannelAction
		/// </summary>
		/// <param name="rabbitMqConnection"></param>
		public LongTermRabbitConnection(IRabbitMqConnection rabbitMqConnection)
		{
			lockObject = new Object();
			this.rabbitMqConnection = rabbitMqConnection;
		}
		
		/// <summary>
		/// Close any existing connections and dispose of unmanaged resources
		/// </summary>
		~LongTermRabbitConnection()
		{
			ShutdownConnection();
		}

		/// <summary>
		/// Close any existing connections and dispose of unmanaged resources
		/// </summary>
		public void Dispose()
		{
			ShutdownConnection();
		}

		/// <summary>
		/// Close any existing connections.
		/// Connections will be re-opened if an action is requested.
		/// </summary>
		public void Reset()
		{
			ShutdownConnection();
		}

		/// <summary>
		/// Perform an action against the RMQ cluster, returning no data
		/// </summary>
		public void WithChannel(Action<IModel> actions)
		{
			lock (lockObject)
			{
				actions(EnsureChannel());
			}
		}

		/// <summary>
		/// Perform an action against the RMQ cluster, returning data
		/// </summary>
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
