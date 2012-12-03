using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Example.Types;
using NUnit.Framework;
using SevenDigital.Messaging.Base;

namespace Messaging.Base.Unit.Tests.Serialisation
{
	[TestFixture]
	public class WhenSerialisingAnObjectWithOneTopLevelInterface
	{
		IMessageSerialiser subject;
		SuperMetaData source;
		string result;

		[SetUp]
		public void With_string_serialised_from_a_source_object ()
		{
			source = new SuperMetaData();
			subject = new MessageSerialiser();
			result = subject.Serialise(source);
		}

		[Test]
		public void Should_serialise_its_properties ()
		{
		}

		[Test]
		public void Should_include_directly_implemented_interface_type ()
		{

		}
	}
}
