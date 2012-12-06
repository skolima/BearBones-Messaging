using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class TypeRouterMultipleInheritanceTests
	{
		ITypeRouter subject;
		IMessageRouter router;

		[SetUp]
		public void SetUp()
		{
			var connection = ConfigurationHelpers.RabbitMqConnectionWithAppConfigSettings();
			router = new RabbitRouter(connection);
			subject = new TypeRouter(router);
		}

		[Test]
		public void When_sending_a_message_with_mulitple_inheritance_should_receive_one_copy_at_base_level()
		{
			subject.BuildRoutes(typeof(IFile));
			
			router.AddDestination("dst");
			router.Link("Example.Types.IMsg", "dst");

			router.Send("Example.Types.IFile", "Hello");

			Assert.That(router.Get("dst"), Is.EqualTo("Hello"));
			Assert.That(router.Get("dst"), Is.Null);
		}

		[TearDown]
		public void teardown()
		{
			((RabbitRouter)router).RemoveRouting(n=>true);
		}
	}
}