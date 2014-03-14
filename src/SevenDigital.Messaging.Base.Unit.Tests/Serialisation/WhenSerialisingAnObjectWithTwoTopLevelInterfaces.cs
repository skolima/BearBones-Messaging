using System;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base.Serialisation;

namespace Messaging.Base.Unit.Tests.Serialisation
{
	[TestFixture]
	public class WhenSerialisingAnObjectWithTwoTopLevelInterfaces
	{
		[Test]
		public void Should_get_an_argument_exception()
		{
			var classWithTwoInterfaces = new ClassWithTwoInterfaces
			{
				CorrelationId = Guid.Parse("05C90FEB-5C10-4179-9FC0-D26DDA5FD1C6"),
				FilePath = "C:\\work\\message",
				HashValue = 123124512
			};

			var exception = Assert.Throws<ArgumentException>(() => (new MessageSerialiser()).Serialise(classWithTwoInterfaces));

			Assert.That(exception.Message, Contains.Substring("Messages must directly implement exactly one interface"));
		}
	}
}