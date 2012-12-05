using Example.Types;
using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.Routing;
using SevenDigital.Messaging.Base.Serialisation;
using StructureMap;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class GetTypeByNameTests
	{
		ITypeRouter typeRouter;
		IMessageRouter messageRouter;
		IMessageSerialiser serialiser;

		[SetUp]
		public void When_setting_up_a_named_destination ()
		{
			typeRouter = Substitute.For<ITypeRouter>();
			messageRouter = Substitute.For<IMessageRouter>();
			serialiser = Substitute.For<IMessageSerialiser>();			
			
			ObjectFactory.ResetDefaults();
			ObjectFactory.Configure(map => {
				map.For<ITypeRouter>().Use(typeRouter);
				map.For<IMessageRouter>().Use(messageRouter);
				map.For<IMessageSerialiser>().Use(serialiser);
			});

			MessagingBase.GetMessage<IMetadataFile>("MyServiceDestination");
		}

		[Test]
		public void Should_get_message_string_from_endpoint ()
		{
			messageRouter.Received().Get("MyServiceDestination");
		}

		[Test]
		public void When_there_is_no_message_should_return_null ()
		{
			messageRouter.Get("MyServiceDestination").Returns((string)null);
			var result = MessagingBase.GetMessage<IMetadataFile>("MyServiceDestination");

			Assert.That(result, Is.Null);
		}

		[Test]
		public void When_a_message_is_available_should_deserialise_and_return_requested_type ()
		{
			messageRouter.Get("MyServiceDestination").Returns("");
			serialiser.Deserialise<IMetadataFile>("").Returns(new SuperMetadata());
			var result = MessagingBase.GetMessage<IMetadataFile>("MyServiceDestination");

			Assert.That(result, Is.InstanceOf<IMetadataFile>());
		}
	}
}
