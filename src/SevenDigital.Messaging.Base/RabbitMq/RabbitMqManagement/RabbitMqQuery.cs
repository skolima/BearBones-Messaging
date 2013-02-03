using System;
using System.Net;
using ServiceStack.Text;
using ShiftIt;
using ShiftIt.Http;

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

            return Uri.TryCreate(HostUri, endpoint, out result) ? GetResponseString(result, 0) : null;
        }

		static string GetResponseString(Uri target, int redirects)
		{
			var rq = new HttpRequestBuilder().Get(target).BasicAuthentication("guest", "guest").Build();
			using (var reader = new HttpClient().Request(rq))
			{
				if (reader.StatusClass == StatusClass.Redirection
					&& reader.Headers.ContainsKey("Location")
					&& redirects < 3)
				{
					return GetResponseString(new Uri(reader.Headers["Location"]), redirects + 1);
				}
				if (reader.StatusClass == StatusClass.Success)
				{
					return reader.BodyReader.ReadStringToLength();
				}
				throw new Exception("Endpoint failed: " + target + "; " + reader.StatusCode);
			}
		}
	}
}