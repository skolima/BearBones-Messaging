using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class TypeRouterMultipleInheritanceTests
	{
		ITypeStructureRouter subject;
		IMessageRouting router;

		[SetUp]
		public void SetUp()
		{
			var connection = ConfigurationHelpers.RabbitMqConnectionWithAppConfigSettings();
			router = new RabbitRouting(connection);
			subject = new TypeStructureRouter(router);
		}

		[Test]
		public void When_sending_a_message_with_mulitple_inheritance_should_receive_one_copy_at_base_level()
		{
			subject.BuildRoutes<IFile>();
			
			router.AddDestination("dst");
			router.Link("Example.Types.IMsg", "dst");

			router.Send("Example.Types.IFile", "Hello");

			Assert.That(router.Get("dst"), Is.EqualTo("Hello"));
			Assert.That(router.Get("dst"), Is.Null);
		}

		[TearDown]
		public void teardown()
		{
			((RabbitRouting)router).RemoveRouting();
		}
	}
}