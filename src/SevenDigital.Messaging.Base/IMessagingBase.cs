namespace SevenDigital.Messaging.Base
{

	public interface IMessagingBase
	{
		void CreateDestination<T>(string destinationName);
		string SendMessage(object messageObject);
		T GetMessage<T>(string destinationName);

        IPendingMessage<T> TryStartMessage<T>(string destinationName);
	}
}