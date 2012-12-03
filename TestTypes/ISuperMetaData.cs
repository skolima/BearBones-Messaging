using System;

namespace Example.Types
{
	public class SuperMetadata: IMetadataFile
	{
		public Guid CorrelationId { get; set; }
		public int HashValue { get; set; }
		public string FilePath { get; set; }
		public string Contents { get; set; }
		public string MetadataName { get; set; }
	}

	public class UltraMegaData : IHash, IPath
	{
		public Guid CorrelationId { get; set; }
		public int HashValue { get; set; }
		public string FilePath { get; set; }
	}

	public class MyPOCO
	{
		public Guid CorrelationId { get; set; }
		public int HashValue { get; set; }
		public string FilePath { get; set; }
	}
}
