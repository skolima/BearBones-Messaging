using System.Configuration;
using SevenDigital.Messaging.Base;
using SevenDigital.Messaging.Base.RabbitMq;
using SevenDigital.Messaging.Base.RabbitMq.RabbitMqManagement;

namespace Messaging.Base.Integration.Tests
{
	public class ConfigurationHelpers
	{
        public static RabbitMqQuery RabbitMqQueryWithConfigSettings()
        {
            var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
            var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
            var username = ConfigurationManager.AppSettings["ApiUsername"];
            var password = ConfigurationManager.AppSettings["ApiPassword"];
            var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

            //return new RabbitMqManagement("http://" + hostUri + ":15672", username, password, vhost); // 3.0+
            return new RabbitMqQuery("http://" + hostUri + ":55672", username, password, vhost); // before RMQ 3
        }

		public static RabbitMqConnection RabbitMqConnectionWithAppConfigSettings()
		{
            var parts = ConfigurationManager.AppSettings["Messaging.Host"].Split('/');
            var hostUri = (parts.Length >= 1) ? (parts[0]) : ("localhost");
            var vhost = (parts.Length >= 2) ? (parts[1]) : ("/");

            return new RabbitMqConnection(hostUri, vhost);
		}
	}
}
