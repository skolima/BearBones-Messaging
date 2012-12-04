using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface IRabbitMqConnection
	{
		ConnectionFactory ConnectionFactory();
		void WithChannel(Action<IModel> actions);
		T GetWithChannel<T>(Func<IModel, T> actions);
		IBasicProperties EmptyBasicProperties();

		string Host { get; }
	}
}