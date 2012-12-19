using System;
using RabbitMQ.Client;

namespace SevenDigital.Messaging.Base.RabbitMq
{
	public interface IChannelAction:IDisposable
	{
		void WithChannel(Action<IModel> actions);
		T GetWithChannel<T>(Func<IModel, T> actions);
	}
}