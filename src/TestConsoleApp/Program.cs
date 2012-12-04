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
			CrossPlatformSignalDispatch.Instance.TerminateEvent += waiter_TerminateEvent;

			var i = 0;
			while(true)
			{
				Thread.Sleep(100);

				if (i++ <= 10) continue;
				Console.Write("Zz");
				i = 0;
			}
		}

		static void waiter_TerminateEvent(object sender, TerminateEventArgs args)
		{
			Console.WriteLine("");
			Console.WriteLine("Got a kill signal ("+args.Signal+"). Will now terminate");
			Environment.Exit(0);
		}
	}
}
