using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
    public class RoutingExample_IMetaData
    {
		ITypeStructureRouter subject;
		IMessageRouting router;

		[SetUp]
		public void A_routing_table_build_from_IMetadataFile ()
		{
			router = Substitute.For<IMessageRouting>();
			subject = new TypeStructureRouter(router);

			subject.BuildRoutes<Example.Types.IMetadataFile>();
		}

		[Test]
		public void Should_create_source_for_IMetadataFile ()
		{
			Assert.Inconclusive();
		}

    }


	public class TypeStructureRouter : ITypeStructureRouter
	{
		public TypeStructureRouter(IMessageRouting router)
		{
		}

		public void BuildRoutes<T>()
		{
		}
	}

	public interface ITypeStructureRouter
	{
		void BuildRoutes<T>();
	}
}