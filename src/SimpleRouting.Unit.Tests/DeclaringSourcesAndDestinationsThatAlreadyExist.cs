using System.Linq;
using Messaging.SimpleRouting;
using NUnit.Framework;

namespace SimpleRouting.Integration.Tests
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
			Assert.That(query.ListDestinations().Count(), Is.EqualTo(initialCount + 1), "Total count of queues");
		}
		
		[Test]
		public void If_I_add_a_source_twice_I_get_one_source_and_no_errors ()
		{
		}

		[Test]
		public void If_I_make_a_link_twice_I_only_get_one_copy_of_each_message ()
		{

		}

		[TearDown]
		public void CleanUp()
		{
			((RabbitRouting)router).RemoveRouting();
		}
	}
}
