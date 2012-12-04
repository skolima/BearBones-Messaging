using System;

namespace SevenDigital.Messaging.Base.Routing
{
	public interface ITypeRouter
	{
		void BuildRoutes<T>();
		void BuildRoutes(Type type);
	}
}