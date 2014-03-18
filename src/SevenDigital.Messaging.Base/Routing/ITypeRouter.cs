using System;

namespace SevenDigital.Messaging.Base.Routing
{
	/// <summary>
	/// Building contract-type routing tree
	/// </summary>
	public interface ITypeRouter
	{
		/// <summary>
		/// Build all dependant types into the messaging server
		/// </summary>
		void BuildRoutes(Type type, string routingKey, ExchangeType exchangeType);
	}
}