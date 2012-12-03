
namespace SevenDigital.Messaging.Base.Serialisation
{
	public interface IMessageSerialiser
	{
		string Serialise<T>(T source);
		T Deserialise<T>(string source);
	}
}