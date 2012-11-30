using Messaging.SimpleRouting.Management;

namespace Messaging.SimpleRouting
{
	public interface IRabbitMqQuery
	{
		RMQueue[] ListQueues();
		RMNode[] ListNodes();
		RMExchange[] ListExchanges();
	}
}