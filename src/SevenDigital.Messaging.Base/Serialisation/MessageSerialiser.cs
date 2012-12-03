using System;
using System.Linq;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Base.Serialisation
{
	public class MessageSerialiser : IMessageSerialiser
	{
		public string Serialise<T>(T messageObject)
		{
			if (messageObject.DirectlyImplementedInterfaces().Count() != 1)
				throw new ArgumentException("Messages must directly implement exactly one interface", "messageObject");

			JsConfig.PreferInterfaces = true;
			return messageObject.ToJson();
		}

		public T Deserialise<T>(string source)
		{
			return (T)JsonSerializer.DeserializeFromString(source, WrapperTypeFor<T>());
		}

		public Type WrapperTypeFor<T>()
		{
			return (typeof(T).IsInterface) ? (DynamicProxy.GetInstanceFor<T>().GetType()) : (typeof(T));
		}
	}
}