using Example.Types;
using NUnit.Framework;
using ServiceStack.Text;
using SevenDigital.Messaging.Base.Serialisation;

namespace Messaging.Base.Unit.Tests.Serialisation
{
	[TestFixture]
	public class ContractStackTests
	{
		string sample = "{\"__type\":\"Example.Types.IMetadataFile, Example.Types\",\"FilePath\":\"path\",\"CorrelationId\":\"05c90feb5c1041799fc0d26dda5fd1c6\",\"HashValue\":123124512,\"Contents\":\"My message contents\",\"MetadataName\":\"Mind the gap\"," +
		                "\"__contracts\":\"" +
		                "Not.A.Real.Type, Example.Types; " +
		                "Example.Types.IMetadataFile, Example.Types; " +
		                "Example.Types.IFile, Example.Types; " +
		                "Example.Types.IHash, Example.Types; " +
		                "Example.Types.IPath, Example.Types; " +
		                "Example.Types.IMsg, Example.Types\"}";

		ContractStack _contractStack;

		[SetUp]
		public void setup ()
		{
			_contractStack = JsonSerializer.DeserializeFromString<ContractStack>(sample);
		}

		[Test]
		public void can_get_contract_stack_from_json_string ()
		{
			Assert.That(_contractStack.__contracts, Is.EqualTo("Not.A.Real.Type, Example.Types; Example.Types.IMetadataFile, Example.Types; Example.Types.IFile, Example.Types; Example.Types.IHash, Example.Types; Example.Types.IPath, Example.Types; Example.Types.IMsg, Example.Types"));
		}

		[Test]
		public void can_get_first_available_real_type ()
		{
			Assert.That(_contractStack.FirstKnownType(),
				Is.EqualTo(typeof(IMetadataFile)));
		}
	}
}
