using System;
using System.Threading;
using SignalHandling.PrivateMonoDerived;

namespace SignalHandling
{
	/// <summary>
	/// Wait for termination events on a background thread.
	/// You should terminate soon after receiving a terminate event.
	/// </summary>
	public class CrossPlatformSignalDispatch
    {

		public event TerminateEvent TerminateEvent;

		public static CrossPlatformSignalDispatch Instance {
			get { 
				return instance ?? (instance = new CrossPlatformSignalDispatch());
			}
		}

		static CrossPlatformSignalDispatch instance;
		static bool RunningUnderPosix
		{
			get
			{
				var p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}
		void TerminateEventSent(int signal)
	    {
		    var handler = TerminateEvent;
			if (handler != null)
			{
				handler(this, new TerminateEventArgs(signal));
			}
			else
			{
				Environment.Exit(signal);
			}
	    }

		private CrossPlatformSignalDispatch()
		{
			if (RunningUnderPosix)
			{
				waitingThread = new Thread(UnixSignalLoop);
				waitingThread.IsBackground = true;
				waitingThread.Start();
			}
			else
			{
				Console.CancelKeyPress += ConsoleCancelKeyPress;
				waitingThread = new Thread(WindowsStdInReader);
				waitingThread.IsBackground = true;
				waitingThread.Start();
			}
		}

		void UnixSignalLoop()
		{
			var signals = new[]{
							new UnixSignal (Signum.SIGINT),  // ^C
							new UnixSignal (Signum.SIGTERM), // kill
							new UnixSignal (Signum.SIGHUP)   // background and drop
						};
			while (waitingThread.IsAlive)
			{
				var which = UnixSignal.WaitAny(signals, -1);
				TerminateEventSent((int) signals[which].Signum);
			}
		}

		void WindowsStdInReader()
		{
			try
			{
				using (var inp = Console.OpenStandardInput()) // this seems to be non-competitive
				{
					int byt;
					while ((byt = inp.ReadByte()) >= 0)// blocking read
					{
						if (byt == 3)  // ctrl-c
						{
							TerminateEventSent((int)Signum.SIGINT);
						}
					}
				}
			}
			catch
			{
				Ignore();
			}
		}

		static void Ignore() { }

		void ConsoleCancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			e.Cancel = true;
			TerminateEventSent((int)Signum.SIGINT);
		}

		readonly Thread waitingThread;
    }

	public delegate void TerminateEvent(object sender, TerminateEventArgs args);

	public class TerminateEventArgs: EventArgs {
		public int Signal { get; set; }

		public TerminateEventArgs(int signal)
		{
			Signal = signal;
		}
	}
}
