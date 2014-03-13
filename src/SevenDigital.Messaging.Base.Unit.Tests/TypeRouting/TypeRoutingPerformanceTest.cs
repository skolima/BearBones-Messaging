using System;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class TypeRoutingPerformanceTest
	{
		ITypeRouter subject;
		IMessageRouter router;

		[Test]
		public void Routing_a_type_1000_times ()
		{
			router = Substitute.For<IMessageRouter>();
			subject = new TypeRouter(router);

			var start = DateTime.Now;
			for (int i = 0; i < 1000; i++)
			{
				subject.BuildRoutes(typeof(Example.Types.SuperMetadata), String.Empty);
			}
			var time = DateTime.Now - start;
			Assert.Pass("Took "+time);
		}
	}
}
