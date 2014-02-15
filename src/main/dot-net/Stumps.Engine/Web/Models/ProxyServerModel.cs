namespace Stumps.Web.Models
{

    public class ProxyServerModel
    {

        public bool AutoStart { get; set; }

        public string ExternalHostName { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

    }

}