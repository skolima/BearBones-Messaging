using System;

namespace SevenDigital.Messaging.Base
{
	public class PendingMessage<T> : IPendingMessage<T>
	{
		public T Message { get; set; }
		public Action Cancel { get; set; }
		public Action Finish { get; set; }
	}
}