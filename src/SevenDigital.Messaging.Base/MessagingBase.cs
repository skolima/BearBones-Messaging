using System;
using System.Collections.Generic;
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

		public static void SendMesssage<T>(T messageObject)
		{
			ObjectFactory.GetInstance<MessagingBase>().Send<T>(messageObject);
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

		void Send<T>(T messageObject)
		{
			var serialised = serialiser.Serialise(messageObject);

			var sourceType = (typeof(T).DirectlyImplementedInterfaces()).Single();
			messageRouter.Send(sourceType.FullName, serialised);
		}
	}
}
