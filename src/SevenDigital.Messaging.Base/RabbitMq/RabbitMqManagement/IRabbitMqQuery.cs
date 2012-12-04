using System;
using System.Net;

namespace SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement
{
	public interface IRabbitMqQuery
	{
		Uri HostUri { get; }
		string VirtualHost { get; }
		NetworkCredential Credentials { get; }

		RMQueue[] ListDestinations();
		RMNode[] ListNodes();
		RMExchange[] ListSources();
	}
}