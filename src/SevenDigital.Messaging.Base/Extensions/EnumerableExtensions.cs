using System.Collections.Generic;

namespace SevenDigital.Messaging.Base
{
	/// <summary>
	/// Extensions for IEnumerable&lt;T&gt;
	/// </summary>
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Returns true if enumerable has exactly one item, false otherwise.
		/// </summary>
		public static bool HasSingle<T>(this IEnumerable<T> src)
		{
			var e = src.GetEnumerator();
			var ok = e.MoveNext();
			ok ^= e.MoveNext();
// ReSharper disable EmptyGeneralCatchClause
			try { e.Reset(); } catch { }
// ReSharper restore EmptyGeneralCatchClause
			return ok;
		}
	}
}
