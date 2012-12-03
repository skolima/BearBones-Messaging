using System;

namespace Example.Types
{
	public interface IMetadataFile : IFile
	{
		string MetadataName { get; set; }
	}

	public interface IFile : IHash, IPath
	{
		string Contents { get; set; }
	}

	public interface IPath: IMsg
	{
		string FilePath { get; set; }
	}

	public interface IHash: IMsg
	{
		int HashValue { get; set; }
	}

	public interface IMsg
	{
		Guid CorrelationId { get; set; }
	}
}