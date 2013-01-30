using System;

namespace SevenDigital.Messaging.Base.Serialisation
{
	public class ContractStack
	{
		const string Marker = "\"__contracts\":\"";


		public static Type FirstKnownType(string message)
		{
			if (string.IsNullOrEmpty(message)) return null;
			var ord = StringComparison.Ordinal;


			int left = message.IndexOf(Marker, ord) + Marker.Length;
			if (left < 0 || (left >= message.Length)) return null;

			while (left < message.Length)
			{
				var right = message.IndexOfAny(new[] { ';', '"' }, left);
				if (right <= left) return null;

				var t = Type.GetType(message.Substring(left, right - left), false);
				if (t != null) return t;

				left = right + 1;
				while (Char.IsWhiteSpace(message[left]))
				{
					left++;
				}
			}
			return null;
		}
	}
}