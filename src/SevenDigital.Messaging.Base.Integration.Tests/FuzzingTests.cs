using System;
using System.Threading;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using StructureMap;

namespace Messaging.Base.Integration.Tests
{
	[TestFixture]
	public class FuzzingTests
	{

		[Test]
		public void can_shutdown_and_restart_connections_on_seperate_threads ()
		{

			new MessagingBaseConfiguration()
				.WithDefaults()
				.WithConnectionFromAppConfig();

			
			var conn = ObjectFactory.GetInstance<IChannelAction>();
			var anyFails = false;

			var b = new Thread(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					conn.Dispose();
					Thread.Sleep(100);
				}
			});
			var a = new Thread(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					if (! conn.GetWithChannel(c => c.IsOpen))
						anyFails = true;
					Thread.Sleep(100);
				}
			});

			a.Start();
			b.Start();

			Assert.That(a.Join(TimeSpan.FromSeconds(20)));
			Assert.That(b.Join(TimeSpan.FromSeconds(20)));
			Assert.False(anyFails);
		}

	}
}