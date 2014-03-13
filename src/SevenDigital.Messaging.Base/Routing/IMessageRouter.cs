using System;

namespace SevenDigital.Messaging.Base.Routing
{
	/// <summary>
	/// Basic actions to drive a RabbitMQ cluster
	/// </summary>
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
		void Link(string sourceName, string destinationName, string routingKey);

		/// <summary>
		/// Route a message between two sources.
		/// </summary>
		void RouteSources(string child, string parent, string routingKey);

		/// <summary>
		/// SendMesssage a message to an established source (will be routed to destinations by key)
		/// </summary>
		void Send(string sourceName, string data, string routingKey);

		/// <summary>
		/// Get a message from a destination. This does not remove the message from the queue.
		/// If a message is returned, it will not be available for another get unless
		/// you use 'Finish' to complete a message and remove from the queue, or 'Cancel'
		/// to release the message back to the queue.
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

		/// <summary>
		/// Cancel a 'Get' by it's tag. The message will remain on the queue and become available for another 'Get'
		/// </summary>
		/// <param name="deliveryTag">Delivery tag as provided by 'Get'</param>
		void Cancel(ulong deliveryTag);

		/// <summary>
		/// Deletes all queues and exchanges created or used by this Router.
		/// </summary>
		void RemoveRouting(Func<string, bool> filter);
	}
}