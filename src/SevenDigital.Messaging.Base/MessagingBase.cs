using System;
using System.Linq;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace SevenDigital.Messaging.Base
{
	public class MessagingBase
	{
		public static void CreateDestination<T>(string destinationName)
		{
			ObjectFactory.GetInstance<MessagingBase>().Create<T>(destinationName);
		}

		public static string SendMessage<T>(T messageObject)
		{
			return ObjectFactory.GetInstance<MessagingBase>().Send<T>(messageObject);
		}

		readonly ITypeRouter typeRouter;
		readonly IMessageRouter messageRouter;
		readonly IMessageSerialiser serialiser;

		public MessagingBase(ITypeRouter typeRouter, IMessageRouter messageRouter, IMessageSerialiser serialiser)
		{
			this.typeRouter = typeRouter;
			this.messageRouter = messageRouter;
			this.serialiser = serialiser;
		}

		void Create<T>(string destinationName)
		{
			typeRouter.BuildRoutes<T>();
			messageRouter.AddDestination(destinationName);
			messageRouter.Link(typeof(T).FullName, destinationName);
		}

		string Send<T>(T messageObject)
		{
			var interfaceTypes = typeof(T).DirectlyImplementedInterfaces().ToList();

			if ( ! interfaceTypes.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "messageObject");

			var sourceType = interfaceTypes.Single();

			var serialised = serialiser.Serialise(messageObject);
			typeRouter.BuildRoutes(sourceType);
			messageRouter.Send(sourceType.FullName, serialised);

			return serialised;
		}
	}
}
