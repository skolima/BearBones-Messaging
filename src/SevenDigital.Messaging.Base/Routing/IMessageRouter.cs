namespace SevenDigital.Messaging.Base.Routing
{
	public interface IMessageRouter
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
		void Link(string sourceName, string destinationName);

		/// <summary>
		/// Route a message between two sources.
		/// </summary>
		void RouteSources(string child, string parent);

		/// <summary>
		/// SendMesssage a message to an established source (will be routed to destinations by key)
		/// </summary>
		void Send(string sourceName, string data);

		/// <summary>
		/// Get a message from a destination. This does not remove the message from the queue.
		/// Use 'Finish' to complete a message and remove from the queue.
		/// </summary>
		string Get(string destinationName, out ulong deliveryTag);

        /// <summary>
        /// Finish a message retrieved by 'Get'.
        /// This will remove the message from the queue
        /// </summary>
        /// <param name="deliveryTag">Delivery tag as provided by 'Get'</param>
        void Finish(ulong deliveryTag);

		/// <summary>
		/// Get a message from a destination, removing it from the queue
		/// </summary>
		string GetAndFinish(string destinationName);

		/// <summary>
		/// Delete all waiting messages from a given destination
		/// </summary>
		void Purge(string destinationName);
	}
}