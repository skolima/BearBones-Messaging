using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface IRabbitMqConnection:IChannelAction
	{
		string Host { get; }
		string VirtualHost { get; }

		ConnectionFactory ConnectionFactory();
	}
}