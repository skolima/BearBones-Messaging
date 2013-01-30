using System;
using System.Linq;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Base.Serialisation
{
	public class MessageSerialiser : IMessageSerialiser
	{
		public string Serialise(object messageObject)
		{
			var type = messageObject.GetType();
			var interfaces = type.DirectlyImplementedInterfaces().ToList();
			if ( ! interfaces.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "messageObject");

			JsConfig.PreferInterfaces = true;
			var str = JsonSerializer.SerializeToString(messageObject, interfaces.Single());
			return str.Insert(str.Length - 1, ",\"__contracts\":\""+InterfaceStack.Of(messageObject)+"\"");
		}

		public T Deserialise<T>(string source)
		{
			JsConfig.PreferInterfaces = true;
			var result = JsonSerializer.DeserializeFromString(source, typeof(object));
			if (result is T) return (T)result;

			return (T)JsonSerializer.DeserializeFromString(source, WrapperTypeFor<T>());
		}

		public object DeserialiseByStack(string source)
		{
			JsConfig.PreferInterfaces = true;

			var bestKnownType = ContractStack.FirstKnownType(source);
			if (bestKnownType == null) 
				throw new Exception("Can't deserialise message, as no matching types are available. Are you missing an assembly reference?");

			return JsonSerializer.DeserializeFromString(source, bestKnownType);
		}


		public Type WrapperTypeFor<T>()
		{
			return (typeof(T).IsInterface) ? (DynamicProxy.GetInstanceFor<T>().GetType()) : (typeof(T));
		}
	}
}