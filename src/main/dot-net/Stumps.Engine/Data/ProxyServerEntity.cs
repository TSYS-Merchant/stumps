namespace Stumps.Data {

    public sealed class ProxyServerEntity {

        public bool AutoStart { get; set; }

        public string ExternalHostName { get; set; }

        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string ProxyId { get; set; }

    }

}
