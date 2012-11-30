using System.Linq;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class DeclaringSourcesAndDestinationsThatAlreadyExist
	{
		private RabbitMqQuery query;
		private IMessageRouting router;
		RabbitMqConnection connection;

		[SetUp]
		public void SetupApi()
		{
			query = RabbitMqQuery.WithConfigSettings();
			connection = RabbitMqConnection.WithAppConfigSettings();
			router = new RabbitRouting(connection);
		}

		[Test]
		public void If_I_add_a_destination_twice_I_get_one_destination_and_no_errors ()
		{
			var initialCount = query.ListDestinations().Count();

			router.AddDestination("B");
			router.AddDestination("B");

			Assert.That(query.ListDestinations().Count(e=>e.name == "B"), Is.EqualTo(1));
			Assert.That(query.ListDestinations().Count(), Is.EqualTo(initialCount + 1), "Total count of destinations");
		}
		
		[Test]
		public void If_I_add_a_source_twice_I_get_one_source_and_no_errors ()
		{
			var initialCount = query.ListSources().Count();

			router.AddSource("S");
			router.AddSource("S");

			Assert.That(query.ListSources().Count(e=>e.name == "S"), Is.EqualTo(1));
			Assert.That(query.ListSources().Count(), Is.EqualTo(initialCount + 1), "Total count of sources");
		}

		[Test]
		public void If_I_make_a_link_twice_I_only_get_one_copy_of_each_message ()
		{
			router.AddSource("src");
			router.AddDestination("dst");

			router.Link("src", "dst");
			router.Link("src", "dst");

			router.Send("src", "Hello");

			Assert.That(router.Get("dst"), Is.EqualTo("Hello"));
			Assert.That(router.Get("dst"), Is.Null);
		}

		[Test]
		public void If_I_make_a_route_between_two_sources_twice_I_only_get_one_copy_of_each_message ()
		{
			router.AddSource("srcA");
			router.AddSource("srcB");
			router.AddDestination("dst");

			router.RouteSources("srcA", "srcB");
			router.RouteSources("srcA", "srcB");

			router.Link("srcB", "dst");
			router.Send("srcA", "Hello");

			Assert.That(router.Get("dst"), Is.EqualTo("Hello"));
			Assert.That(router.Get("dst"), Is.Null);
		}

		[TearDown]
		public void CleanUp()
		{
			((RabbitRouting)router).RemoveRouting();
		}
	}
}
