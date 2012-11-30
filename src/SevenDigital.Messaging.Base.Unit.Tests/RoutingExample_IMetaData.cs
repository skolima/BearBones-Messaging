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
		public void A_routing_table_build_from_IMetadataFile()
		{
			router = Substitute.For<IMessageRouting>();

			subject = new TypeStructureRouter(router);

			subject.BuildRoutes<Example.Types.IMetadataFile>();
		}

		[Test]
		[TestCase("Example.Types.IMetadataFile")]
		[TestCase("Example.Types.IFile")]
		[TestCase("Example.Types.IHash")]
		[TestCase("Example.Types.IPath")]
		[TestCase("Example.Types.IMsg")]
		public void Should_create_source_for_each_interface_type(string interfaceFullType)
		{
			router.Received().AddSource(interfaceFullType);
		}

		[Test]
		public void Should_link_IMetadataFile_to_IFile_with_IFile_as_parent()
		{
			router.Received().RouteSources("Example.Types.IMetadataFile", "Example.Types.IFile");
		}

		[Test]
		public void Should_link_IFile_to_IHash_with_IHash_as_parent()
		{
			router.Received().RouteSources("Example.Types.IFile", "Example.Types.IHash");
		}

		[Test]
		[TestCase("Example.Types.IMetadataFile")]
		[TestCase("Example.Types.IHash")]
		[TestCase("Example.Types.IPath")]
		[TestCase("Example.Types.IMsg")]
		public void Should_not_link_IMetadataFile_to_itself_indirect_antecendants(string antecendant)
		{
			router.DidNotReceive().RouteSources("Example.Types.IMetadataFile", antecendant);
		}

		[Test]
		public void Should_link_IFile_to_IPath_with_IPath_as_parent()
		{
			router.Received().RouteSources("Example.Types.IFile", "Example.Types.IPath");
		}

		[Test]
		public void Should_link_IHash_to_IPath_with_IMsg_as_parent()
		{
			router.Received().RouteSources("Example.Types.IHash", "Example.Types.IMsg");
		}

		[Test]
		public void Should_link_IPath_to_IPath_with_IMsg_as_parent()
		{
			router.Received().RouteSources("Example.Types.IPath", "Example.Types.IMsg");
		}
	}
}