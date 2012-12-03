using ServiceStack.Text;

namespace SevenDigital.Messaging.Base
{
	public class MessageSerialiser : IMessageSerialiser
	{
		public string Serialise<T>(T source)
		{
			JsConfig.PreferInterfaces = true;
			return source.ToJson();
		}
	}
}