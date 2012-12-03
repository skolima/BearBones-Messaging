using System;
using System.Collections.Generic;
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

		void AddSourcesAndRoute(Type type)
		{
			foreach (var interfaceType in DirectlyImplementedInterfaces(type))
			{
				router.AddSource(interfaceType.FullName);
				router.RouteSources(type.FullName, interfaceType.FullName);
				AddSourcesAndRoute(interfaceType);
			}
		}

		readonly Dictionary<Type, Type[]> interfaces = new Dictionary<Type, Type[]>();
		IEnumerable<Type> DirectlyImplementedInterfaces(Type type)
		{
			//return type.GetInterfaces().Where(i => !type.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i)));
			if (interfaces.ContainsKey(type)) return interfaces[type];
			lock (interfaces)
			{
				interfaces.Add(type, type.GetInterfaces().Where(i => !type.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i))).ToArray());
				return interfaces[type];
			}
		}
	}
}