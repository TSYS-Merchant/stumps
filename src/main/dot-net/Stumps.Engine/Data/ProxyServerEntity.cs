namespace Stumps.Data
{

    /// <summary>
    ///     A class that represents the persisted form proxy server's configuration.
    /// </summary>
    public sealed class ProxyServerEntity
    {

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically start the proxy server after it is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server should automatically start after it is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        ///     Gets or sets the name of the host name for the remote server.
        /// </summary>
        /// <value>
        ///     The host name of the remote server.
        /// </value>
        public string ExternalHostName { get; set; }

        /// <summary>
        ///     Gets or sets the TCP port the proxy server listens to for traffic.
        /// </summary>
        /// <value>
        ///     The TCP port the proxy server listens to for traffic.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the proxy unique identifier for the proxy server.
        /// </summary>
        /// <value>
        /// The proxy unique identifier for the proxy server.
        /// </value>
        public string ProxyId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote server requires an SSL connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the remote server requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }

    }

}