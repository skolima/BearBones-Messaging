using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SignalHandling
{
	class UnixSignal : WaitHandle
	{
		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int Mono_Posix_RuntimeIsShuttingDown();

		
		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_ToSignum")]
		private static extern int ToSignum(int value, out Signum rval);
		public static bool TryToSignum(int value, out Signum rval)
		{
			return ToSignum(value, out rval) == 0;
		}
		public static Signum ToSignum(int value)
		{
			Signum result;
			if (ToSignum(value, out result) == -1)
			{
				throw new ArgumentException("value "+value+" is not an acceptable signum");
			}
			return result;
		}
		
		public static RealTimeSignum ToRealTimeSignum(int offset)
		{
			return new RealTimeSignum(offset);
		}
		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromSignum")]
		private static extern int FromSignum(Signum value, out int rval);
		public static bool TryFromSignum(Signum value, out int rval)
		{
			return FromSignum(value, out rval) == 0;
		}
		public static int FromSignum(Signum value)
		{
			int result;
			if (FromSignum(value, out result) == -1)
			{
				throw new ArgumentException("value "+value+" is not an acceptable signum");
			}
			return result;
		}
		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_FromRealTimeSignum")]
		private static extern int FromRealTimeSignum(int offset, out int rval);
		public static int FromRealTimeSignum(RealTimeSignum sig)
		{
			int result;
			if (FromRealTimeSignum(sig.Offset, out result) == -1)
			{
				throw new ArgumentException("sig.Offset "+sig.Offset+" is not an acceptable offset");
			}
			return result;
		}

		private struct SignalInfo
		{
			public int signum;
			public int count;
			public int read_fd;
			public int write_fd;
			public int have_handler;
			public int pipecnt;
			public IntPtr handler;
		}
		private int signum;
		private IntPtr signal_info;
		private static UnixSignal.Mono_Posix_RuntimeIsShuttingDown ShuttingDown = new UnixSignal.Mono_Posix_RuntimeIsShuttingDown(UnixSignal.RuntimeShuttingDownCallback);
		public Signum Signum
		{
			get
			{
				if (this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is a RealTimeSignum");
				}
				return ToSignum(this.signum);
			}
		}
		public RealTimeSignum RealTimeSignum
		{
			get
			{
				if (!this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is not a RealTimeSignum");
				}
				return ToRealTimeSignum(this.signum - UnixSignal.GetSIGRTMIN());
			}
		}
		public bool IsRealTimeSignal
		{
			get
			{
				this.AssertValid();
				int sIGRTMIN = UnixSignal.GetSIGRTMIN();
				return sIGRTMIN != -1 && this.signum >= sIGRTMIN;
			}
		}
		private unsafe UnixSignal.SignalInfo* Info
		{
			get
			{
				this.AssertValid();
				return (UnixSignal.SignalInfo*)((void*)this.signal_info);
			}
		}
		public bool IsSet
		{
			get
			{
				return this.Count > 0;
			}
		}
		public unsafe int Count
		{
			get
			{
				return this.Info->count;
			}
			set
			{
				Interlocked.Exchange(ref this.Info->count, value);
			}
		}
		public UnixSignal(Signum signum)
		{
			this.signum = FromSignum(signum);
			this.signal_info = UnixSignal.install(this.signum);
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ArgumentException("Unable to handle signal", "signum");
			}
		}
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_install", SetLastError = true)]
		private static extern IntPtr install(int signum);
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_uninstall")]
		private static extern int uninstall(IntPtr info);
		private static int RuntimeShuttingDownCallback()
		{
			return (!Environment.HasShutdownStarted) ? 0 : 1;
		}
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Unix_UnixSignal_WaitAny")]
		private static extern int WaitAny(IntPtr[] infos, int count, int timeout, UnixSignal.Mono_Posix_RuntimeIsShuttingDown shutting_down);
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMIN")]
		internal static extern int GetSIGRTMIN();
		[DllImport("MonoPosixHelper", CallingConvention = CallingConvention.Cdecl, EntryPoint = "Mono_Posix_SIGRTMAX")]
		internal static extern int GetSIGRTMAX();
		private void AssertValid()
		{
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}
		public unsafe bool Reset()
		{
			int num = Interlocked.Exchange(ref this.Info->count, 0);
			return num != 0;
		}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.signal_info == IntPtr.Zero)
			{
				return;
			}
			UnixSignal.uninstall(this.signal_info);
			this.signal_info = IntPtr.Zero;
		}
		public override bool WaitOne()
		{
			return this.WaitOne(-1, false);
		}
		public override bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.WaitOne((int)num, exitContext);
		}
		public override bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			this.AssertValid();
			if (exitContext)
			{
				throw new InvalidOperationException("exitContext is not supported");
			}
			return UnixSignal.WaitAny(new UnixSignal[]
			{
				this
			}, millisecondsTimeout) == 0;
		}
		public static int WaitAny(UnixSignal[] signals)
		{
			return UnixSignal.WaitAny(signals, -1);
		}
		public static int WaitAny(UnixSignal[] signals, TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1L || num > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return UnixSignal.WaitAny(signals, (int)num);
		}
		public static int WaitAny(UnixSignal[] signals, int millisecondsTimeout)
		{
			if (signals == null)
			{
				throw new ArgumentNullException("signals");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			IntPtr[] array = new IntPtr[signals.Length];
			for (int i = 0; i < signals.Length; i++)
			{
				array[i] = signals[i].signal_info;
				if (array[i] == IntPtr.Zero)
				{
					throw new InvalidOperationException("Disposed UnixSignal");
				}
			}
			return UnixSignal.WaitAny(array, array.Length, millisecondsTimeout, UnixSignal.ShuttingDown);
		}
	}
}
