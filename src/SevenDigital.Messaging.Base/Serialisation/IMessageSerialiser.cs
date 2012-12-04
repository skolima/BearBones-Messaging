
namespace SevenDigital.Messaging.Base.Serialisation
{
	public interface IMessageSerialiser
	{
		string Serialise(object source);
		T Deserialise<T>(string source);
	}
}