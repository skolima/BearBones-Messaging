using System;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;

namespace Messaging.Base.Unit.Tests.Serialisation
{
	[TestFixture]
	public class InterfaceStackTests
	{
		[Test]
		public void Should_get_correct_stack_for_complex_types ()
		{
			var source = new SuperMetadata();

			var result = InterfaceStack.Of(source);
			Console.WriteLine(result);
			Assert.That(result,
				Is.EqualTo("Example.Types.IMetadataFile, Example.Types;" +
				           "Example.Types.IFile, Example.Types;" +
				           "Example.Types.IHash, Example.Types;" +
				           "Example.Types.IPath, Example.Types;" +
				           "Example.Types.IMsg, Example.Types"));
		}
	}
}
