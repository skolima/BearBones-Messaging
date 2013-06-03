using System;

namespace SevenDigital.Messaging.Base
{
	/// <summary>
	/// A received message instance
	/// </summary>
	public class PendingMessage<T> : IPendingMessage<T>
	{
		/// <summary>Message on queue</summary>
		public T Message { get; set; }

		/// <summary>Action to cancel and return message to queue</summary>
		public Action Cancel { get; set; }

		/// <summary>Action to complete message and remove from queue</summary>
		public Action Finish { get; set; }
	}
}