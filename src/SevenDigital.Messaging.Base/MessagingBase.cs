using SevenDigital.Messaging.Base.Routing;
using StructureMap;

namespace SevenDigital.Messaging.Base
{
	public class MessagingBase
	{
		public static void CreateDestination<T>(string destinationName)
		{
			ObjectFactory.GetInstance<MessagingBase>().Create<T>(destinationName);
		}

		public static void Send<T>(T messageObject)
		{
			ObjectFactory.GetInstance<MessagingBase>().SendMessage<T>(messageObject);
		}

		readonly ITypeRouter typeRouter;
		readonly IMessageRouter messageRouter;

		public MessagingBase(ITypeRouter typeRouter, IMessageRouter messageRouter)
		{
			this.typeRouter = typeRouter;
			this.messageRouter = messageRouter;
		}
		void Create<T>(string destinationName)
		{
			typeRouter.BuildRoutes<T>();
			messageRouter.AddDestination(destinationName);
			messageRouter.Link(typeof(T).FullName, destinationName);
		}
		void SendMessage<T>(T messageObject)
		{
		}
	}
}
