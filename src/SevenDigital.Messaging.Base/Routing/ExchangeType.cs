namespace SevenDigital.Messaging.Base.Routing
{
	/// <summary>
	/// Specifies how exchange behaviour differs in regard to routing keys
	/// </summary>
	public enum ExchangeType
	{
		/// <summary>
		/// Exchange that delivers to binding exactly matching the routing key
		/// </summary>
		Direct,
		/// <summary>
		/// Exchange that ignores routing keys
		/// </summary>
		Fanout,
		/// <summary>
		/// Exchange that allows partial matching on the routing key
		/// </summary>
		Topic
	}
}