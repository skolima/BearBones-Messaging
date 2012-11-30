using System;
using RabbitMQ.Client;

namespace Messaging.SimpleRouting
{
	public interface IRabbitMqConnection
	{
		ConnectionFactory ConnectionFactory();
		void WithChannel(Action<IModel> actions);
		T GetWithChannel<T>(Func<IModel, T> actions);
		IBasicProperties EmptyBasicProperties();
	}
}