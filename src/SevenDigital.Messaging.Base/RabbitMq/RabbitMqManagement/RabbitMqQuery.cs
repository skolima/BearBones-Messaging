using System;
using System.Net;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement
{
	/// <summary>
	/// Deafult RMQ query
	/// </summary>
	public class RabbitMqQuery : IRabbitMqQuery
	{
		/// <summary>
		/// RabbitMQ cluster's management uri
		/// </summary>
		public Uri HostUri { get; private set; }

		/// <summary>
		/// Virtual host to use, where applicable
		/// </summary>
		public string VirtualHost { get; private set; }

		/// <summary>
		/// Log-in credentials for RabbitMQ management API
		/// </summary>
		public NetworkCredential Credentials { get; private set; }

		/// <summary>
		/// Use `MessagingBaseConfiguration` and get an IRabbitMqQuery from ObjectFactory.
		/// </summary>
		public RabbitMqQuery(Uri managementApiHost, NetworkCredential credentials)
		{
			HostUri = managementApiHost;
			Credentials = credentials;
		}
		
		/// <summary>
		/// Use `MessagingBaseConfiguration` and get an IRabbitMqQuery from ObjectFactory.
		/// </summary>
		public RabbitMqQuery(string hostUri, string username, string password, string virtualHost = "/")
			: this(new Uri(hostUri), new NetworkCredential(username, password))
		{
			VirtualHost = (virtualHost.StartsWith("/")) ? (virtualHost) : ("/" + virtualHost);
		}

		/// <summary>
		/// List all Destination queue in the given virtual host.
		/// Equivalent to /api/queues/vhost
		/// </summary>
		public RMQueue[] ListDestinations()
		{
			return JsonSerializer.DeserializeFromString<RMQueue[]>(Get("/api/queues" + VirtualHost));
		}

		/// <summary>
		/// List all nodes attached to the cluster.
		/// Equivalent to /api/nodes
		/// </summary>
		public RMNode[] ListNodes()
		{
			return JsonSerializer.DeserializeFromString<RMNode[]>(Get("/api/nodes"));
		}

		/// <summary>
		/// List all Source exchanges in the given virtual host
		/// Equivalent to /api/exchanges/vhost
		/// </summary>
		public RMExchange[] ListSources()
		{
			return JsonSerializer.DeserializeFromString<RMExchange[]>(Get("/api/exchanges" + VirtualHost));
		}

		string Get(string endpoint)
		{
			Uri result;

			return Uri.TryCreate(HostUri, endpoint, out result) ? GetResponseString(result) : null;
		}

		static string GetResponseString(Uri target)
		{
			using (var webclient = new WebClient())
			{
				webclient.UseDefaultCredentials = true;
				webclient.Credentials = new NetworkCredential("guest", "guest");
				return webclient.DownloadString(target);
			}
		}
	}
}