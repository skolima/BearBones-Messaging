using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;
using SevenDigital.Messaging.Base.RabbitMq;

namespace SevenDigital.Messaging.Base.Routing
{
	/// <summary>
	/// Very simple synchronous message routing over RabbitMq
	/// </summary>
	public class RabbitRouter : IMessageRouter
	{
		readonly ISet<string> queues;
		readonly ISet<string> exchanges;
		readonly IDictionary noOptions;
		readonly IChannelAction messagingChannel;

		/// <summary>
		/// Create a new router from config settings
		/// </summary>
		public RabbitRouter(IChannelAction messagingChannel)
		{
			this.messagingChannel = messagingChannel;
			queues = new HashSet<string>();
			exchanges = new HashSet<string>();
			noOptions = new Dictionary<string,string>();
		}

		/// <summary>
		/// Deletes all queues and exchanges created or used by this Router.
		/// </summary>
		public void RemoveRouting (Func<string, bool> filter)
		{
			MessagingBase.ResetCaches();
			messagingChannel.WithChannel(channel =>
				{
					foreach (var queue in queues.Where(filter))
					{
						channel.QueueDelete(queue);
					}

					foreach (var exchange in exchanges.Where(filter))
					{
						channel.ExchangeDelete(exchange);
					}
				});

			queues.Clear();
			exchanges.Clear();
		}

		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node send messages over links that share a routing key.
		/// </summary>
		public void AddSource(string name)
		{
			messagingChannel.WithChannel(channel => channel.ExchangeDeclare(name, "direct", true, false, noOptions));
			exchanges.Add(name);
		}
		
		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node sends messages to all its links
		/// </summary>
		public void AddBroadcastSource(string className)
		{
			messagingChannel.WithChannel(channel => channel.ExchangeDeclare(className, "fanout", true, false, noOptions));
			exchanges.Add(className);
		}

		/// <summary>
		/// Add a new node where messages can be picked up
		/// </summary>
		public void AddDestination(string name)
		{
			messagingChannel.WithChannel(channel => channel.QueueDeclare(name, true, false, false, noOptions));
			queues.Add(name);
		}

		/// <summary>
		/// Create a link between a source node and a destination node by a routing key
		/// </summary>
		public void Link(string sourceName, string destinationName)
		{
			messagingChannel.WithChannel(channel => channel.QueueBind(destinationName, sourceName, ""));
		}

		/// <summary>
		/// Route a message between two sources.
		/// </summary>
		public void RouteSources(string child, string parent)
		{
			if (parent == child) throw new ArgumentException("Can't bind a source to itself");
			messagingChannel.WithChannel(channel => channel.ExchangeBind(parent, child, ""));
		}

		/// <summary>
		/// Send a message to an established source (will be routed to destinations by key)
		/// </summary>
		public void Send(string sourceName, string data)
		{
			messagingChannel.WithChannel(channel => channel.BasicPublish(
				sourceName, "", false, false, EmptyBasicProperties(),
				Encoding.UTF8.GetBytes(data))
				);
		}

		/// <summary>
		/// Get a message from a destination. This removes the message from the destination
		/// </summary>
		public string Get(string destinationName)
		{
			var result = messagingChannel.GetWithChannel(channel => {
				var rs = channel.BasicGet(destinationName, false);
				if (rs == null) return null;
				channel.BasicAck(rs.DeliveryTag, false);
				return rs;
			});
			return result == null ? null : Encoding.UTF8.GetString(result.Body);
		}

		/// <summary>
		/// Delete all waiting messages from a given destination
		/// </summary>
		public void Purge(string destinationName)
        {
            messagingChannel.WithChannel(channel => channel.QueuePurge(destinationName));
        }		
		public IBasicProperties EmptyBasicProperties()
		{
			return new BasicProperties();
		}
	}
}