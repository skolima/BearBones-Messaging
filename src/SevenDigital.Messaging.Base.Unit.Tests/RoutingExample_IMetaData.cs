using NSubstitute;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture, Ignore("NYI")]
	public class RoutingExample_IMetaData
	{
		ITypeStructureRouter subject;
		IMessageRouting router;

		[SetUp]
		public void A_routing_table_build_from_IMetadataFile()
		{
			router = Substitute.For<IMessageRouting>();

			subject = new TypeStructureRouter(router);

			subject.BuildRoutes<Example.Types.IMetadataFile>();
		}

		[Test]
		public void Should_create_source_for_IMetadataFile()
		{
			router.Received().AddSource("Example.Types.IMetadataFile");
		}

		[Test]
		public void Should_create_source_for_IFile()
		{
			router.Received().AddSource("Example.Types.IFile");
		}

		[Test]
		public void Should_link_IMetadataFile_to_IFile_with_IFile_as_parent()
		{
			router.Received().RouteSources("Example.Types.IFile", "Example.Types.IMetadataFile", "");
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