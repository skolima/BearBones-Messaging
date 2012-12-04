namespace SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement
{
	public interface IRabbitMqQuery
	{
		RMQueue[] ListDestinations();
		RMNode[] ListNodes();
		RMExchange[] ListSources();
	}
}