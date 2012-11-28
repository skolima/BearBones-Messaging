using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using Messaging.SimpleRouting.Management;
using RabbitMQ.Client;
using ServiceStack.Text;

namespace Messaging.SimpleRouting
{
    public class RabbitMqApi
    {
        readonly string virtualHost;
        readonly Uri managementApiHost;
        readonly NetworkCredential credentials;
        readonly string slashHost;

        /// <summary>
        /// Uses app settings: "Messaging.Host", "ApiUsername", "ApiPassword"
        /// </summary>
        public static RabbitMqApi WithConfigSettings()
        {
            var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
            var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
            var username = ConfigurationManager.AppSettings["ApiUsername"];
            var password = ConfigurationManager.AppSettings["ApiPassword"];
            var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

            return new RabbitMqApi("http://" + hostUri + ":15672", username, password, vhost); // 3.0+
            //return new RabbitMqApi("http://" + hostUri + ":55672", username, password, vhost); // before RMQ 3
        }

        public RabbitMqApi(Uri managementApiHost, NetworkCredential credentials)
        {
            this.managementApiHost = managementApiHost;
            this.credentials = credentials;
        }

        public RabbitMqApi(string hostUri, string username, string password, string virtualHost = "/")
            : this(new Uri(hostUri), new NetworkCredential(username, password))
        {
            this.virtualHost = virtualHost;
            slashHost = (virtualHost.StartsWith("/")) ? (virtualHost) : ("/" + virtualHost);
        }

        public RMQueue[] ListQueues()
        {
            using (var stream = Get("/api/queues" + slashHost))
                return JsonSerializer.DeserializeFromStream<RMQueue[]>(stream);
        }

        public RMNode[] ListNodes()
        {
            using (var stream = Get("/api/nodes"))
                return JsonSerializer.DeserializeFromStream<RMNode[]>(stream);
        }

        public RMExchange[] ListExchanges()
        {
            using (var stream = Get("/api/exchanges" + slashHost))
                return JsonSerializer.DeserializeFromStream<RMExchange[]>(stream);
        }

        public Stream Get(string endpoint)
        {
            Uri result;

            if (Uri.TryCreate(managementApiHost, endpoint, out result))
            {
                var webRequest = WebRequest.Create(result);
                webRequest.Credentials = credentials;
                return webRequest.GetResponse().GetResponseStream();
            }

            return null;
        }

        public ConnectionFactory ConnectionFactory()
        {
            return new ConnectionFactory
                {
                    Protocol = Protocols.FromEnvironment(),
                    HostName = managementApiHost.Host,
                    VirtualHost = virtualHost
                };
        }

        public void PurgeQueue(RMQueue queue)
        {
            var factory = ConnectionFactory();

            var conn = factory.CreateConnection();
            var ch = conn.CreateModel();
            ch.QueuePurge(queue.name);
            ch.Close();
            conn.Close();
        }

	    public void DeleteExchange(string exchangeName)
	    {
			var factory = ConnectionFactory();
		    using (var conn = factory.CreateConnection())
			{
				var ch = conn.CreateModel();
				ch.ExchangeDelete(exchangeName);
				ch.Close();
		    }
	    }

	    public void DeleteQueue(string queueName)
        {
            var factory = ConnectionFactory();

            var conn = factory.CreateConnection();
            var ch = conn.CreateModel();
            ch.QueueDelete(queueName);

            ch.Close();
            conn.Close();
        }

        public void AddQueue(string queueName)
        {
            var factory = ConnectionFactory();

            var conn = factory.CreateConnection();
            var ch = conn.CreateModel();
            ch.QueueDeclare(queueName, true, false, false, new Dictionary<string, string>());
            ch.Close();
            conn.Close();
        }

		public void WithChannel(Action<IModel> actions)
		{
			var factory = ConnectionFactory();
			using (var conn = factory.CreateConnection())
			using (var channel = conn.CreateModel())
			{
				actions(channel);
				channel.Close();
				conn.Close();
			}
		}

		public T GetWithChannel<T>(Func<IModel, T> actions)
		{
			var factory = ConnectionFactory();
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