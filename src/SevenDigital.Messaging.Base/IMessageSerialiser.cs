
namespace SevenDigital.Messaging.Base
{
	public interface IMessageSerialiser
	{
		string Serialise<T>(T source);
	}
}