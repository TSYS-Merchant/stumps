namespace Stumps.Web.Models
{

    /// <summary>
    ///     A class that represents the model for a proxy server.
    /// </summary>
    public class ProxyServerModel
    {

        /// <summary>
        /// Gets or sets a value indicating whether the proxy server automatically starts with the service.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server automatically starts with the service; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        ///     Gets or sets the name of the server.
        /// </summary>
        /// <value>
        ///     The name of the server.
        /// </value>
        public string ServerName { get; set; }

        /// <summary>
        ///     Gets or sets the external host name served by the proxy.
        /// </summary>
        /// <value>
        ///     The name of external host name served by the proxy.
        /// </value>
        public string ExternalHostName { get; set; }

        /// <summary>
        ///     Gets or sets the port that is used to listen for traffic.
        /// </summary>
        /// <value>
        ///     The port used to listen for traffic.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote host requires SSL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the remote host requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }

        

    }

}