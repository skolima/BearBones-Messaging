using System;

namespace SevenDigital.Messaging.Base.Routing
{
	public class TypeRouter : ITypeRouter
	{
		readonly IMessageRouter router;

		public TypeRouter(IMessageRouter router)
		{
			this.router = router;
		}

		public void BuildRoutes(Type type)
		{
			if (type.IsInterface) router.AddSource(type.FullName);
			AddSourcesAndRoute(type);
		}

		void AddSourcesAndRoute(Type type)
		{
			foreach (var interfaceType in type.DirectlyImplementedInterfaces())
			{
				router.AddSource(interfaceType.FullName);
				router.RouteSources(type.FullName, interfaceType.FullName);
				AddSourcesAndRoute(interfaceType);
			}
		}
	}
}