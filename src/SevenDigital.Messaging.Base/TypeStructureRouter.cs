using System;
using System.Linq;

namespace SevenDigital.Messaging.Base
{
	public class TypeStructureRouter : ITypeStructureRouter
	{
		private readonly IMessageRouting router;

		public TypeStructureRouter(IMessageRouting router)
		{
			this.router = router;
		}

		public void BuildRoutes<T>()
		{
			Type type = typeof(T);

			router.AddSource(type.FullName);
			AddSourcesAndRoute(type);
		}

		private void AddSourcesAndRoute(Type type)
		{
			var interfaceTypes = type.GetInterfaces().Where(i => !type.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i))).ToArray();
			
			if (interfaceTypes.Any())
			{
				foreach (var interfaceType in interfaceTypes)
				{
					router.AddSource(interfaceType.FullName);
					router.RouteSources(type.FullName, interfaceType.FullName, "");
					AddSourcesAndRoute(interfaceType);
				}
			}
		}
	}
}