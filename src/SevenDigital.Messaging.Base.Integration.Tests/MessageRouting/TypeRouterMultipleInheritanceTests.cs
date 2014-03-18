﻿using System;
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
			var longTermConnection = ConfigurationHelpers.ChannelWithAppConfigSettings();
			var shortTermConnection = ConfigurationHelpers.FreshConnectionFromAppConfig();
			router = new RabbitRouter(longTermConnection, shortTermConnection);
			subject = new TypeRouter(router);
		}

		[Test]
		public void When_sending_a_message_with_mulitple_inheritance_should_receive_one_copy_at_base_level()
		{
			subject.BuildRoutes(typeof(IFile), String.Empty, ExchangeType.Direct);
			
			router.AddDestination("dst");
			router.Link("Example.Types.IMsg", "dst", String.Empty);

			router.Send("Example.Types.IFile", "Hello", String.Empty);

			Assert.That(router.GetAndFinish("dst"), Is.EqualTo("Hello"));
			Assert.That(router.GetAndFinish("dst"), Is.Null);
		}

		[TearDown]
		public void teardown()
		{
			((RabbitRouter)router).RemoveRouting(n=>true);
		}
	}
}