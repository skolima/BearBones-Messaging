using System;

namespace SevenDigital.Messaging.Base.Routing
{
	/// <summary>
	/// Default type router
	/// </summary>
	public class TypeRouter : ITypeRouter
	{
		readonly IMessageRouter router;

		/// <summary>
		/// Create a type router to drive the given message router.
		/// You don't need to do this yourself -- Use `MessagingBaseConfiguration`
		/// </summary>
		public TypeRouter(IMessageRouter router)
		{
			this.router = router;
		}

		/// <summary>
		/// Build all dependant types into the messaging server
		/// </summary>
		/// <param name="type"></param>
		/// <param name="routingKey"></param>
		public void BuildRoutes(Type type, string routingKey)
		{
			if (type.IsInterface) router.AddSource(type.FullName, ExchangeType.Direct);
			AddSourcesAndRoute(type, routingKey);
		}

		void AddSourcesAndRoute(Type type, string routingKey)
		{
			foreach (var interfaceType in type.DirectlyImplementedInterfaces())
			{
				router.AddSource(interfaceType.FullName, ExchangeType.Direct);
				router.RouteSources(type.FullName, interfaceType.FullName, routingKey);
				AddSourcesAndRoute(interfaceType, routingKey);
			}
		}
	}
}