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
			var connection = ConfigurationHelpers.ChannelWithAppConfigSettings();
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

			Assert.That(GetAndFinish(router, "dst"), Is.EqualTo("Hello"));
			Assert.That(GetAndFinish(router, "dst"), Is.Null);
		}

		string GetAndFinish(IMessageRouter messageRouter, string dst)
		{
			ulong tag;
            var str = messageRouter.Get(dst, out tag);
            if (str != null) messageRouter.Finish(tag);
            return str;
		}

		[TearDown]
		public void teardown()
		{
			((RabbitRouter)router).RemoveRouting(n=>true);
		}
	}
}