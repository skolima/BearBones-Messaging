namespace Example.Types
{
	public interface IMetadataFile : IFile
	{
	}

	public interface IFile : IHash, IPath
	{
	}

	public interface IPath: IMsg
	{
	}

	public interface IHash: IMsg
	{
	}

	public interface IMsg
	{
	}
}