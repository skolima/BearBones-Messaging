using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Unit.Tests
{
	[TestFixture]
	public class ContractNameTests
	{
		[Test]
		public void Should_get_full_name_of_single_interface_from_contract_name_method ()
		{
			var instance = new Example.Types.SuperMetadata();
			var result = MessagingBase.ContractTypeName(instance);

			Assert.That(result, Is.EqualTo("Example.Types.IMetadataFile"));
		}
	}
}
