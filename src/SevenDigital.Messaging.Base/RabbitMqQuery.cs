using System;
using System.IO;
using System.Net;
using ServiceStack.Text;
using SevenDigital.Messaging.Base.Management;

namespace SevenDigital.Messaging.Base
{
	public class RabbitMqQuery : IRabbitMqQuery
	{
        readonly Uri managementApiHost;
        readonly NetworkCredential credentials;
        readonly string slashHost;


        public RabbitMqQuery(Uri managementApiHost, NetworkCredential credentials)
        {
            this.managementApiHost = managementApiHost;
            this.credentials = credentials;
        }

        public RabbitMqQuery(string hostUri, string username, string password, string virtualHost = "/")
            : this(new Uri(hostUri), new NetworkCredential(username, password))
        {
            slashHost = (virtualHost.StartsWith("/")) ? (virtualHost) : ("/" + virtualHost);
        }

        public RMQueue[] ListDestinations()
        {
            using (var stream = Get("/api/queues" + slashHost))
                return JsonSerializer.DeserializeFromStream<RMQueue[]>(stream);
        }

        public RMNode[] ListNodes()
        {
            using (var stream = Get("/api/nodes"))
                return JsonSerializer.DeserializeFromStream<RMNode[]>(stream);
        }

        public RMExchange[] ListSources()
        {
            using (var stream = Get("/api/exchanges" + slashHost))
                return JsonSerializer.DeserializeFromStream<RMExchange[]>(stream);
        }

        Stream Get(string endpoint)
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
    }
}