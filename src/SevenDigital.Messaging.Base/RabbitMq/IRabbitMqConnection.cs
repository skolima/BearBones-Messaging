using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface IRabbitMqConnection
	{
		string Host { get; }
		string VirtualHost { get; }

		ConnectionFactory ConnectionFactory();
		void WithChannel(Action<IModel> actions);
		T GetWithChannel<T>(Func<IModel, T> actions);
		IBasicProperties EmptyBasicProperties();
	}
}