using System;
using System.Threading;
using SignalHandling;

namespace TestConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Waiting for signal. Press ^C or send a SIGTERM (kill $pid)");
			var waiter = new CrossPlatformSignalDispatch();
			waiter.TerminateEvent += waiter_TerminateEvent;

			while(true)
			{
				Thread.Sleep(250);
			}
		}

		static void waiter_TerminateEvent(object sender, TerminateEventArgs args)
		{
			Console.WriteLine("Got a kill signal. Will now terminate");
			Environment.Exit(0);
		}
	}
}
