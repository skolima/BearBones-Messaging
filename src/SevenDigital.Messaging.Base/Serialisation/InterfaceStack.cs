using System;
using System.Collections.Generic;
using System.Linq;

namespace SevenDigital.Messaging.Base.Serialisation
{
	public class InterfaceStack
	{
		public static string Of(object source)
		{
			var set = new List<Type>();
			Interfaces(source.DirectlyImplementedInterfaces(), set);
			return 
				string.Join("; ",
				            set.Select(type => Shorten(type.AssemblyQualifiedName))
					);
		}

		static string Shorten(string assemblyQualifiedName)
		{
			return string.Join(",", assemblyQualifiedName.Split(',').Take(2));
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