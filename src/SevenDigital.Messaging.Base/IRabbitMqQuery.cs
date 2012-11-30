using SevenDigital.Messaging.Base.Management;

namespace SevenDigital.Messaging.Base
{
	public interface IRabbitMqQuery
	{
		RMQueue[] ListDestinations();
		RMNode[] ListNodes();
		RMExchange[] ListSources();
	}
}