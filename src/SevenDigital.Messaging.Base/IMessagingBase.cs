using System;

namespace SevenDigital.Messaging.Base
{
	/// <summary>
	/// Core messaging functions
	/// </summary>
	public interface IMessagingBase
	{
		/// <summary>
		/// Ensure a destination exists, and bind it to the exchanges for the given type
		/// </summary>
		void CreateDestination<T>(string destinationName);
		
		/// <summary>
		/// Ensure a destination exists, and bind it to the exchanges for the given type
		/// </summary>
		void CreateDestination(Type sourceMessage, string destinationName);

		/// <summary>
		/// Send a message to all bound destinations.
		/// Returns serialised form of the message object.
		/// </summary>
		string SendMessage(object messageObject);

		/// <summary>
		/// Poll for a waiting message. Returns default(T) if no message.
		/// </summary>
		T GetMessage<T>(string destinationName);

		/// <summary>
		/// Try to start handling a waiting message.
		/// The message may be acknowledged or cancelled to finish reception.
		/// </summary>
		IPendingMessage<T> TryStartMessage<T>(string destinationName);
	}
}