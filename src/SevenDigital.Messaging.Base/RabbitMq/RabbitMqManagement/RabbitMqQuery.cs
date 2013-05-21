using System;
using System.Net;
using ServiceStack.Text;

namespace SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement
{
	public class RabbitMqQuery : IRabbitMqQuery
	{
		public Uri HostUri { get; private set; }
		public string VirtualHost { get; private set; }
		public NetworkCredential Credentials { get; private set; }

		public RabbitMqQuery(Uri managementApiHost, NetworkCredential credentials)
		{
			HostUri = managementApiHost;
			Credentials = credentials;
		}

		public RabbitMqQuery(string hostUri, string username, string password, string virtualHost = "/")
			: this(new Uri(hostUri), new NetworkCredential(username, password))
		{
			VirtualHost = (virtualHost.StartsWith("/")) ? (virtualHost) : ("/" + virtualHost);
		}

		public RMQueue[] ListDestinations()
		{
			return JsonSerializer.DeserializeFromString<RMQueue[]>(Get("/api/queues" + VirtualHost));
		}

		public RMNode[] ListNodes()
		{
			return JsonSerializer.DeserializeFromString<RMNode[]>(Get("/api/nodes"));
		}

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