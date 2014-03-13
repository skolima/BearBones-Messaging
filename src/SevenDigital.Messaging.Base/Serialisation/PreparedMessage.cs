using System;
using System.Text;

namespace SevenDigital.Messaging.Base.Serialisation
{
	/// <summary>
	/// A pre-serialised message
	/// </summary>
	public class PreparedMessage : IPreparedMessage
	{
		private const string _serializedVersionPrefix = "V2=";
		private const string _delimiter = "|";
		private const string _errorMessage = "Invalid prepared message";
		readonly string _typeName;
		readonly string _message;
		private readonly string _routingKey;

		/// <summary>
		/// Create a new prepared message from a type name and message string
		/// </summary>
		public PreparedMessage(string typeName, string message, string routingKey)
		{
			_typeName = typeName;
			_message = message;
			_routingKey = routingKey;
		}

		/// <summary>
		/// Restore a prepared message from bytes
		/// </summary>
		public static PreparedMessage FromBytes(byte[] bytes)
		{
			var concatMsg = Encoding.UTF8.GetString(bytes);
			if (concatMsg.StartsWith(_serializedVersionPrefix))
			{
				var msgNoVersion = concatMsg.Substring(_serializedVersionPrefix.Length);
				var parts = msgNoVersion.Split(new[] { _delimiter }, 3, StringSplitOptions.None);
				if (parts.Length < 3) throw new Exception(_errorMessage);
				return new PreparedMessage(parts[0], parts[2], parts[1]);
			}
			else
			{
				var parts = concatMsg.Split(new[] {_delimiter}, 2, StringSplitOptions.None);
				if (parts.Length < 2) throw new Exception(_errorMessage);
				return new PreparedMessage(parts[0], parts[1], String.Empty);
			}
		}

		/// <summary>
		/// Return a storable list of bytes representing the message
		/// </summary>
		public byte[] ToBytes()
		{
			return Encoding.UTF8.GetBytes(_serializedVersionPrefix + _typeName + _delimiter + _routingKey + _delimiter + _message);
		}

		/// <summary>
		/// Return routable type name
		/// </summary>
		public string TypeName()
		{
			return _typeName;
		}

		/// <summary>
		/// Return serialised message string
		/// </summary>
		public string SerialisedMessage()
		{
			return _message;
		}

		/// <summary>
		/// Routing key for this message
		/// </summary>
		public string RoutingKey { get { return _routingKey; } }
	}
}