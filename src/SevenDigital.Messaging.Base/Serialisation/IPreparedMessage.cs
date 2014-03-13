namespace SevenDigital.Messaging.Base.Serialisation
{
	/// <summary>
	/// Contract for serialised messages
	/// </summary>
	public interface IPreparedMessage
	{
		/// <summary>
		/// Return a storable list of bytes representing the message
		/// </summary>
		byte[] ToBytes();

		/// <summary>
		/// Return routable type name
		/// </summary>
		string TypeName();

		/// <summary>
		/// Return serialised message string
		/// </summary>
		string SerialisedMessage();

		/// <summary>
		/// Routing key for this message
		/// </summary>
		string RoutingKey { get; }
	}
}