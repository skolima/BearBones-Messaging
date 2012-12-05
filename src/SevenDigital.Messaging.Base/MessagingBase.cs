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

		public static string SendMessage<T>(T messageObject)
		{
			return ObjectFactory.GetInstance<MessagingBase>().Send(messageObject);
		}

		public static T GetMessage<T>(string destinationName)
		{
			return ObjectFactory.GetInstance<MessagingBase>().Get<T>(destinationName);
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
			RouteSource(typeof(T));
			messageRouter.AddDestination(destinationName);
			messageRouter.Link(typeof(T).FullName, destinationName);
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
		/// Ensure that routes are rebuild on next SendMessage or CreateDestination.
		/// </summary>
		internal static void ResetRouteCache()
		{
			lock (RouteCache)
			{
				RouteCache.Clear();
			}
		}

		string Send(object messageObject)
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

		T Get<T>(string destinationName)
		{
			var messageString = messageRouter.Get(destinationName);
			return (messageString == null) ? (default(T)) : (serialiser.Deserialise<T>(messageString));
		}
	}
}
