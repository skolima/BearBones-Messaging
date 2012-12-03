// ReSharper disable CheckNamespace
namespace System
{
	using Collections.Generic;
	using Linq;

	public static class TypeExtensions
	{
		public static IEnumerable<Type> DirectlyImplementedInterfaces(this Type type)
		{
			return type.GetInterfaces().Where(i => !type.GetInterfaces().Any(i2 => i2.GetInterfaces().Contains(i)));
		}

		public static IEnumerable<Type> DirectlyImplementedInterfaces<T>(this T src)
		{
			var type = src.GetType();
			return DirectlyImplementedInterfaces(type);
		}
	}
}
