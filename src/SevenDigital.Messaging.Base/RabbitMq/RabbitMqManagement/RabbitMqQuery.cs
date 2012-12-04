using System;
using System.IO;
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
            using (var stream = Get("/api/queues" + VirtualHost))
                return JsonSerializer.DeserializeFromStream<RMQueue[]>(stream);
        }

        public RMNode[] ListNodes()
        {
            using (var stream = Get("/api/nodes"))
                return JsonSerializer.DeserializeFromStream<RMNode[]>(stream);
        }

        public RMExchange[] ListSources()
        {
            using (var stream = Get("/api/exchanges" + VirtualHost))
                return JsonSerializer.DeserializeFromStream<RMExchange[]>(stream);
        }

		Stream Get(string endpoint)
        {
            Uri result;

            if (Uri.TryCreate(HostUri, endpoint, out result))
            {
                var webRequest = WebRequest.Create(result);
                webRequest.Credentials = Credentials;
                return webRequest.GetResponse().GetResponseStream();
            }

            return null;
        }
    }
}