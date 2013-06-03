using System;
using System.Collections.Generic;
using System.Linq;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace SevenDigital.Messaging.Base
{
	/// <summary>
	/// Default messaging base.
	/// </summary>
	public class MessagingBase : IMessagingBase
	{
		readonly ITypeRouter typeRouter;
		readonly IMessageRouter messageRouter;
		readonly IMessageSerialiser serialiser;

		/// <summary>
		/// Create with `ObjectFactory.GetInstance&lt;IMessagingBase&gt;()`
		/// </summary>
		public MessagingBase(ITypeRouter typeRouter, IMessageRouter messageRouter, IMessageSerialiser serialiser)
		{
			this.typeRouter = typeRouter;
			this.messageRouter = messageRouter;
			this.serialiser = serialiser;
		}

		/// <summary>
		/// Get the contract name of an object instance
		/// </summary>
		public static string ContractTypeName(object instance)
		{
			return ContractTypeName(instance.GetType());
		}
		
		/// <summary>
		/// Get the contract name of a type
		/// </summary>
		public static string ContractTypeName(Type type)
		{
			if (type.IsInterface) return type.FullName;

			var interfaceTypes = type.DirectlyImplementedInterfaces().ToList();

			if (!interfaceTypes.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "type");

			return interfaceTypes.Single().FullName;
		}

		/// <summary>
		/// Ensure a destination exists, and bind it to the exchanges for the given type
		/// </summary>
		public void CreateDestination<T>(string destinationName)
		{
			CreateDestination(typeof(T), destinationName);
		}

		/// <summary>
		/// Ensure a destination exists, and bind it to the exchanges for the given type
		/// </summary>
		public void CreateDestination(Type sourceType, string destinationName)
		{
			RouteSource(sourceType);
			messageRouter.AddDestination(destinationName);
			messageRouter.Link(sourceType.FullName, destinationName);
		}

		/// <summary>
		/// Send a message to all bound destinations.
		/// Returns serialised form of the message object.
		/// </summary>
		public string SendMessage(object messageObject)
		{
			var interfaceTypes = messageObject.GetType().DirectlyImplementedInterfaces().ToList();

			if (!interfaceTypes.HasSingle())
				throw new ArgumentException("Messages must directly implement exactly one interface", "messageObject");

			var sourceType = interfaceTypes.Single();
			var serialised = serialiser.Serialise(messageObject);

			RouteSource(sourceType);
			messageRouter.Send(sourceType.FullName, serialised);

			return serialised;
		}

		/// <summary>
		/// Poll for a waiting message. Returns default(T) if no message.
		/// </summary>
		public T GetMessage<T>(string destinationName)
		{
			var messageString = messageRouter.GetAndFinish(destinationName);

			if (messageString == null) return default(T);

			try
			{
				return (T)serialiser.DeserialiseByStack(messageString);
			}
			catch
			{
				return serialiser.Deserialise<T>(messageString);
			}
		}

		/// <summary>
		/// Try to start handling a waiting message.
		/// The message may be acknowledged or cancelled to finish reception.
		/// </summary>
		public IPendingMessage<T> TryStartMessage<T>(string destinationName)
		{
			ulong deliveryTag;
			var messageString = messageRouter.Get(destinationName, out deliveryTag);

			if (messageString == null) return null;

			T message;
			try
			{
				message = (T)serialiser.DeserialiseByStack(messageString);
			}
			catch
			{
				message = serialiser.Deserialise<T>(messageString);
			}

			return new PendingMessage<T>
			{
				Message = message,
				Cancel = () => messageRouter.Cancel(deliveryTag),
				Finish = () => messageRouter.Finish(deliveryTag)
			};
		}

		static readonly IDictionary<Type, RateLimitedAction> RouteCache = new Dictionary<Type, RateLimitedAction>();
		void RouteSource(Type routeType)
		{
			lock (RouteCache)
			{
				if (!RouteCache.ContainsKey(routeType))
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
