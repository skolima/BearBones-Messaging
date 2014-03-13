﻿using System;
using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;
using SevenDigital.Messaging.Base.Routing;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class DeclaringSourcesAndDestinationsThatAlreadyExist
	{
		private RabbitMqQuery query;
		private IMessageRouter router;
		IChannelAction connection;

		[SetUp]
		public void SetupApi()
		{
			query = ConfigurationHelpers.RabbitMqQueryWithConfigSettings();
			connection = ConfigurationHelpers.ChannelWithAppConfigSettings();
			var shortTermConnection = ConfigurationHelpers.FreshConnectionFromAppConfig();
			router = new RabbitRouter(connection, shortTermConnection);
		}

		[Test]
		public void If_I_add_a_destination_twice_I_get_one_destination_and_no_errors ()
		{
			router.AddDestination("B");
			router.AddDestination("B");

			Assert.That(query.ListDestinations().Count(e=>e.name == "B"), Is.EqualTo(1));
		}
		
		[Test]
		public void If_I_add_a_source_twice_I_get_one_source_and_no_errors ()
		{
			router.AddSource("S");
			router.AddSource("S");

			Assert.That(query.ListSources().Count(e=>e.name == "S"), Is.EqualTo(1));
		}

		[Test]
		public void If_I_make_a_link_twice_I_only_get_one_copy_of_each_message ()
		{
			router.AddSource("src");
			router.AddDestination("dst");

			router.Link("src", "dst", String.Empty);
			router.Link("src", "dst", String.Empty);

			router.Send("src", "Hello", String.Empty);

			Assert.That(router.GetAndFinish("dst"), Is.EqualTo("Hello"));
			Assert.That(router.GetAndFinish("dst"), Is.Null);
		}

		[Test]
		public void If_I_make_a_route_between_two_sources_twice_I_only_get_one_copy_of_each_message ()
		{
			router.AddSource("srcA");
			router.AddSource("srcB");
			router.AddDestination("dst");

			router.RouteSources("srcA", "srcB", "");
			router.RouteSources("srcA", "srcB", "");

			router.Link("srcB", "dst", String.Empty);
			router.Send("srcA", "Hello", String.Empty);

			Assert.That(router.GetAndFinish("dst"), Is.EqualTo("Hello"));
			Assert.That(router.GetAndFinish("dst"), Is.Null);
		}

		[TearDown]
		public void CleanUp()
		{
			((RabbitRouter)router).RemoveRouting(n=>true);
		}
	}
}
