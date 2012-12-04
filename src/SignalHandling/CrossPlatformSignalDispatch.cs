using System;
using System.Threading;

namespace SignalHandling
{
	/// <summary>
	/// Wait for termination events on a background thread.
	/// You should terminate soon after receiving a terminate event.
	/// </summary>
	public class CrossPlatformSignalDispatch
    {
	    public static bool RunningUnderMono { get { return Type.GetType("Mono.Runtime") != null; } }

		public event TerminateEvent TerminateEvent;

	    void TerminateEventSent(int Signal)
	    {
		    var handler = TerminateEvent;
			if (handler != null)
			{
				handler(this, new TerminateEventArgs(Signal));
			}
			else
			{
				Environment.Exit(Signal);
			}
	    }

		public CrossPlatformSignalDispatch()
		{
			if (RunningUnderMono)
			{
				waitingThread = new Thread(UnixSignalLoop);
				waitingThread.IsBackground = true;
				waitingThread.Start();
			}
			else
			{
				Console.CancelKeyPress += Console_CancelKeyPress;
			}
		}

		void UnixSignalLoop()
		{
			var signals = new[]{
							new UnixSignal (Signum.SIGINT),  // ^C
							new UnixSignal (Signum.SIGTERM), // kill
							new UnixSignal (Signum.SIGHUP), // background and drop
						};
			while (true)
			{
				int which = UnixSignal.WaitAny(signals, -1);
				TerminateEventSent((int)signals[which].Signum);
			}
		}

		void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
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
