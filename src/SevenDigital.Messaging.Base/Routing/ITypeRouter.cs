﻿using System;

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
		/// <param name="type"></param>
		/// <param name="routingKey"></param>
		void BuildRoutes(Type type, string routingKey);
	}
}