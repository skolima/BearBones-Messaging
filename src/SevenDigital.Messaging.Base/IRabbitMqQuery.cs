using Messaging.SimpleRouting.Management;

namespace Messaging.SimpleRouting
{
	public interface IRabbitMqQuery
	{
		RMQueue[] ListDestinations();
		RMNode[] ListNodes();
		RMExchange[] ListSources();
	}
}