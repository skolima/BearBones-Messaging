using System;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Routing;

namespace Messaging.Base.Integration.Tests.MessageRouting
{
	[TestFixture]
	public class RetryingMessagesTests
	{
		ITypeRouter typeRouter;
		IMessageRouter subject;

		[SetUp]
		public void SetUp()
		{
			var longTermConnection = ConfigurationHelpers.ChannelWithAppConfigSettings();
			var shortTermConnection = ConfigurationHelpers.FreshConnectionFromAppConfig();
			subject = new RabbitRouter(longTermConnection, shortTermConnection);

			typeRouter = new TypeRouter(subject);
			typeRouter.BuildRoutes(typeof(IFile), String.Empty);

			subject.AddDestination("dst");
			subject.Link("Example.Types.IMsg", "dst", String.Empty);
			subject.Send("Example.Types.IFile", "Hello", String.Empty);
		}

		[Test]
		public void cant_get_a_message_twice_even_if_its_not_finished()
		{
			ulong tag1, tag2;
			Assert.That(subject.Get("dst", out tag1), Is.EqualTo("Hello"));
			Assert.That(subject.Get("dst", out tag2), Is.Null);

			subject.Finish(tag1);
		}

		[Test]
		public void can_cancel_a_message_making_it_available_again()
		{
			ulong tag1, tag2;
			Assert.That(subject.Get("dst", out tag1), Is.EqualTo("Hello"));
			Assert.That(subject.Get("dst", out tag2), Is.Null);

			subject.Cancel(tag1);
			Assert.That(subject.Get("dst", out tag2), Is.EqualTo("Hello"));

			subject.Finish(tag2);
		}

		[Test]
		public void cancelled_messages_return_to_the_head_of_the_queue()
		{

			ulong tag1, tag2;
			Assert.That(subject.Get("dst", out tag1), Is.EqualTo("Hello"));
			subject.Send("Example.Types.IFile", "SecondMessage", String.Empty);

			subject.Cancel(tag1);
			Assert.That(subject.Get("dst", out tag1), Is.EqualTo("Hello"));
			Assert.That(subject.Get("dst", out tag2), Is.EqualTo("SecondMessage"));

			subject.Finish(tag1);
			subject.Finish(tag2);
		}


		[Test]
		public void with_two_messages_waiting_and_one_is_in_progress_the_other_can_be_picked_up()
		{
			subject.Send("Example.Types.IFile", "SecondMessage", String.Empty);

			ulong tag1, tag2;
			Assert.That(subject.Get("dst", out tag1), Is.EqualTo("Hello"));
			Assert.That(subject.Get("dst", out tag2), Is.EqualTo("SecondMessage"));

			subject.Finish(tag1);
			subject.Finish(tag2);
		}
	}
}