using System;
using System.Linq;

namespace SevenDigital.Messaging.Base.Serialisation
{
	public class ContractStack
	{
		public string __contracts { get; set; }

		public Type FirstKnownType()
		{
			if (__contracts == null) return null;

			var typespecs = __contracts.Split(';');
			return typespecs
				.Select(typespec => Type.GetType(typespec.Trim(), /*throw on error:*/false, /*ignore case:*/true))
				.FirstOrDefault(t => t != null);
		}
	}
}