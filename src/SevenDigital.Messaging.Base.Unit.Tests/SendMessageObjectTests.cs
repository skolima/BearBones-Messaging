using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Example.Types;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using StructureMap;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class SendMessageObjectTests
	{
		ITypeRouter typeRouter;
		IMessageRouter messageRouter;

		[SetUp]
		public void When_setting_up_a_named_destination ()
		{
			typeRouter = Substitute.For<ITypeRouter>();
			messageRouter = Substitute.For<IMessageRouter>();
			ObjectFactory.Configure(map => {
				map.For<ITypeRouter>().Use(typeRouter);
				map.For<IMessageRouter>().Use(messageRouter);
			});

			MessagingBase.Send(new MetadataMessage());
		}


	}

	public class MetadataMessage: IMetadataFile
	{
		public MetadataMessage()
		{
			CorrelationId = Guid.NewGuid();
		}
		public Guid CorrelationId { get; set; }
		public int HashValue { get; set; }
		public string FilePath { get; set; }
		public string Contents { get; set; }
		public string MetadataName { get; set; }
	}
}
