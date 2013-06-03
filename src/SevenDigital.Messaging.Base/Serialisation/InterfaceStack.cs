using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SevenDigital.Messaging.Base.Serialisation
{
	/// <summary>
	/// Helper class for reading the definition stack of interfaces.
	/// </summary>
	public class InterfaceStack
	{
		/// <summary>
		/// Return a string implementation of 
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static string Of(object source)
		{
			var set = new List<Type>();
			Interfaces(source.DirectlyImplementedInterfaces(), set);
			
			var sb = new StringBuilder();
			for (int i = 0; i < set.Count; i++)
			{
				var type = set[i];
				if (i>0) sb.Append(";");
				sb.Append(Shorten(type.AssemblyQualifiedName));
			}
			return sb.ToString();
		}

		static string Shorten(string assemblyQualifiedName)
		{
			var idx = assemblyQualifiedName.IndexOf(", Version", StringComparison.Ordinal);
			return idx < 0 ? assemblyQualifiedName : assemblyQualifiedName.Substring(0, idx);
		}

		static void Interfaces(IEnumerable<Type> interfaces, ICollection<Type> set)
		{
			var types = interfaces as Type[] ?? interfaces.ToArray();
			foreach (var type in types.Where(type => !set.Contains(type)))
			{
				set.Add(type);
			}
			foreach (var type in types)
			{
				Interfaces(type.DirectlyImplementedInterfaces(), set);
			}
		}
	}
}