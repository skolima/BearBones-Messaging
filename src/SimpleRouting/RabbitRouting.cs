using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Framing.v0_9_1;

namespace Messaging.SimpleRouting
{
	/// <summary>
	/// Very simple synchronous message routing over RabbitMq
	/// </summary>
	public class RabbitRouting : IMessageRouting
	{
		readonly RabbitMqApi api;
		readonly List<string> queues;
		readonly List<string> exchanges;
		readonly IDictionary noOptions;

		/// <summary>
		/// Create a new router from config settings
		/// </summary>
		public RabbitRouting()
		{
			api = RabbitMqApi.WithConfigSettings();
			queues = new List<string>();
			exchanges = new List<string>();
			noOptions = new Dictionary<string,string>();
		}

		/// <summary>
		/// Deletes all queues and exchanges created or used by this Router.
		/// </summary>
		public void RemoveRouting ()
		{
			foreach (var queue in queues)
			{
				api.DeleteQueue(queue);
			}
			
			foreach (var exchange in exchanges)
			{
				api.DeleteExchange(exchange);
			}

			queues.Clear();
			exchanges.Clear();
		}
		
		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node send messages over links that share a routing key.
		/// </summary>
		public void AddSource(string name)
		{
			WithChannel(channel => channel.ExchangeDeclare(name, "direct", true, false, noOptions));
			exchanges.Add(name);
		}
		
		/// <summary>
		/// Add a new node to which messages can be sent.
		/// This node sends messages to all its links
		/// </summary>
		public void AddBroadcastSource(string className)
		{
			WithChannel(channel => channel.ExchangeDeclare(className, "fanout", true, false, noOptions));
			exchanges.Add(className);
		}

		/// <summary>
		/// Add a new node where messages can be picked up
		/// </summary>
		public void AddDestination(string name)
		{
			WithChannel(channel => channel.QueueDeclare(name, true, false, false, noOptions));
			queues.Add(name);
		}

		/// <summary>
		/// Create a link between a source node and a destination node by a routing key
		/// </summary>
		public void Link(string sourceName, string destinationName, string routingKey)
		{
			WithChannel(channel => channel.QueueBind(destinationName, sourceName, routingKey));
		}

		/// <summary>
		/// Route a message between two sources.
		/// </summary>
		public void Route(string parent, string child, string routingKey)
		{
			WithChannel(channel => channel.ExchangeBind(parent, child, routingKey));
		}

		/// <summary>
		/// Send a message to an established source (will be routed to destinations by key)
		/// </summary>
		public void Send(string sourceName, string routingKey, string data)
		{
			WithChannel(channel => channel.BasicPublish(sourceName, routingKey, false, false, new BasicProperties(), Encoding.UTF8.GetBytes(data)));
		}

		/// <summary>
		/// Get a message from a destination. This removes the message from the destination
		/// </summary>
		public string Get(string destinationName)
		{
			var result = GetWithChannel(channel => {
				var rs = channel.BasicGet(destinationName, false);
				if (rs == null) return null;
				channel.BasicAck(rs.DeliveryTag, false);
				return rs;
			});
			return result == null ? null : Encoding.UTF8.GetString(result.Body);
		}

		private void WithChannel(Action<IModel> actions)
		{
			var factory = api.ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				actions(channel);
				channel.Close();
				conn.Close();
			}
		}

		private T GetWithChannel<T>(Func<IModel, T> actions)
		{
			var factory = api.ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				var result = actions(channel);
				channel.Close();
				conn.Close();
				return result;
			}
		}
	}
}