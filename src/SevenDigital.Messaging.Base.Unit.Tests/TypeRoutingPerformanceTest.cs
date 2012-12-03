using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class TypeRoutingPerformanceTest
	{
		ITypeStructureRouter subject;
		IMessageRouting router;

		[Test]
		public void Routing_a_type_1000_times ()
		{
			router = Substitute.For<IMessageRouting>();
			subject = new TypeStructureRouter(router);

			var start = DateTime.Now;
			for (int i = 0; i < 10000; i++)
			{
				subject.BuildRoutes<Example.Types.SuperMetaData>();
			}
			var time = DateTime.Now - start;
			Console.WriteLine(time);
		}
	}
}
