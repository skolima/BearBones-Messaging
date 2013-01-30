using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace SevenDigital.Messaging.Base
{
	public interface IMessagingBase
	{
		void CreateDestination<T>(string destinationName);
		string SendMessage(object messageObject);
		T GetMessage<T>(string destinationName);
		object GetMessage(string destinationName);
	}

	public class MessagingBase : IMessagingBase
	{
		readonly ITypeRouter typeRouter;
		readonly IMessageRouter messageRouter;
		readonly IMessageSerialiser serialiser;

		public MessagingBase(ITypeRouter typeRouter, IMessageRouter messageRouter, IMessageSerialiser serialiser)
		{
			this.typeRouter = typeRouter;
			this.messageRouter = messageRouter;
			this.serialiser = serialiser;
		}
		
		public static string ContractTypeName(object instance)
		{
			return ContractTypeName(instance.GetType());
		}
		public static string ContractTypeName(Type type)
		{
			if (type.IsInterface) return type.FullName;

			var interfaceTypes = type.DirectlyImplementedInterfaces().ToList();

			if ( ! interfaceTypes.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "type");

			return interfaceTypes.Single().FullName;
		}

		public void CreateDestination<T>(string destinationName)
		{
			RouteSource(typeof(T));
			messageRouter.AddDestination(destinationName);
			messageRouter.Link(typeof(T).FullName, destinationName);
		}

		public string SendMessage(object messageObject)
		{
			var interfaceTypes = messageObject.GetType().DirectlyImplementedInterfaces().ToList();

			if ( ! interfaceTypes.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "messageObject");

			var sourceType = interfaceTypes.Single();
			var serialised = serialiser.Serialise(messageObject);

			RouteSource(sourceType);
			messageRouter.Send(sourceType.FullName, serialised);

			return serialised;
		}

		public T GetMessage<T>(string destinationName)
		{
			var messageString = messageRouter.Get(destinationName);
			return (messageString == null) ? (default(T)) : (serialiser.Deserialise<T>(messageString));
		}
		
		public object GetMessage(string destinationName)
		{
			var messageString = messageRouter.Get(destinationName);
			return (messageString == null) ? (null) : (serialiser.DeserialiseByStack(messageString));
		}

		static readonly IDictionary<Type, RateLimitedAction> RouteCache = new Dictionary<Type, RateLimitedAction>();
		void RouteSource(Type routeType)
		{
			lock (RouteCache)
			{
				if (! RouteCache.ContainsKey(routeType))
				{
					RouteCache.Add(routeType, RateLimitedAction.Of(() => typeRouter.BuildRoutes(routeType)));
				}
			}
			RouteCache[routeType].YoungerThan(TimeSpan.FromMinutes(1));
		}
		
		/// <summary>
		/// Ensure that routes and connections are rebuild on next SendMessage or CreateDestination.
		/// </summary>
		public static void ResetCaches()
		{
			lock (RouteCache)
			{
				RouteCache.Clear();
			}
			var channelAction = ObjectFactory.TryGetInstance<IChannelAction>();
			if (channelAction is LongTermRabbitConnection)
			{
				((LongTermRabbitConnection)channelAction).Reset();
			}
		}
	}
}
