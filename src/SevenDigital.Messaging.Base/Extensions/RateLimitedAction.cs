using System;

namespace SevenDigital.Messaging.Base
{
	public class RateLimitedAction
	{
		private readonly Action action;
		private DateTime lastRecall;

		private RateLimitedAction(Action actionToPerform)
		{
			action = actionToPerform;
			lastRecall = DateTime.MinValue;
		}

		public static RateLimitedAction Of(Action actionToPerform)
		{
			return new RateLimitedAction(actionToPerform);
		}

		public void YoungerThan(TimeSpan Age)
		{
			if ((DateTime.Now - lastRecall) <= Age) return;
			action();
			lastRecall = DateTime.Now;
		}
	}
}
