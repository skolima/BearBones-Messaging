using System;
using System.Linq;

namespace SevenDigital.Messaging.Base
{
	public class TypeStructureRouter : ITypeStructureRouter
	{
		readonly IMessageRouting router;

		public TypeStructureRouter(IMessageRouting router)
		{
			this.router = router;
		}

		public void BuildRoutes<T>()
		{
			var type = typeof(T);

			if (type.IsInterface) router.AddSource(type.FullName);
			AddSourcesAndRoute(type);
		}

		private void AddSourcesAndRoute(Type type)
		{
			var interfaceTypes = type.GetInterfaces().Where(i => !type.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i))).ToArray();

			if (!interfaceTypes.Any()) return;

			foreach (var interfaceType in interfaceTypes)
			{
				router.AddSource(interfaceType.FullName);
				router.RouteSources(type.FullName, interfaceType.FullName);
				AddSourcesAndRoute(interfaceType);
			}
		}
	}
}