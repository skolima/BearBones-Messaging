namespace SevenDigital.Messaging.Base
{
	public interface IMessageRouting
	{
		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node send messages over links that share a routing key.
		/// </summary>
		void AddSource(string name);

		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node sends messages to all its links
		/// </summary>
		void AddBroadcastSource(string className);

		/// <summary>
		/// Add a new node where messages can be picked up
		/// </summary>
		void AddDestination(string name);

		/// <summary>
		/// Create a link between a source node and a destination node by a routing key
		/// </summary>
		void Link(string sourceName, string destinationName, string routingKey);

		/// <summary>
		/// Route a message between two sources.
		/// </summary>
		void RouteSources(string parent, string child, string routingKey);

		/// <summary>
		/// Send a message to an established source (will be routed to destinations by key)
		/// </summary>
		void Send(string sourceName, string routingKey, string data);

		/// <summary>
		/// Get a message from a destination. This removes the message from the destination
		/// </summary>
		string Get(string destinationName);

		/// <summary>
		/// Delete all waiting messages from a given destination
		/// </summary>
		void Purge(string destinationName);
	}
}