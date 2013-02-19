using System;

namespace SevenDigital.Messaging.Base
{
	public interface IPendingMessage<out T>
	{
		/// <summary>Message on queue</summary>
		T Message { get; }

		/// <summary>Action to cancel and return message to queue</summary>
		Action Cancel { get; }

		/// <summary>Action to complete message and remove from queue</summary>
		Action Finish { get; }
	}
}