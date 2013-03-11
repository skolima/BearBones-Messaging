using System;

namespace SevenDigital.Messaging.Base
{

	public interface IMessagingBase
	{
		void CreateDestination<T>(string destinationName);
		void CreateDestination(Type sourceMessage, string destinationName);
		string SendMessage(object messageObject);
		T GetMessage<T>(string destinationName);

		IPendingMessage<T> TryStartMessage<T>(string destinationName);
	}
}