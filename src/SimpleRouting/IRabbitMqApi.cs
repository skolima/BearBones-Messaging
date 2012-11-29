using System;
using Messaging.SimpleRouting.Management;
using RabbitMQ.Client;

namespace Messaging.SimpleRouting
{
	public interface IRabbitMqApi
	{
		RMQueue[] ListQueues();
		RMNode[] ListNodes();
		RMExchange[] ListExchanges();
		void PurgeQueue(RMQueue queue);
		void DeleteExchange(string exchangeName);
		void DeleteQueue(string queueName);
		void WithChannel(Action<IModel> actions);
		T GetWithChannel<T>(Func<IModel, T> actions);
	}
}