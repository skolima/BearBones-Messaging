namespace SevenDigital.Messaging.Base
{
	public class MessageSerialiser : IMessageSerialiser
	{
		public string Serialise<T>(T source)
		{
			return "woop";
		}
	}
}