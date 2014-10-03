namespace Stumps.Web.Api
{

    /// <summary>
    ///     A class that represents the model for a Stumps server.
    /// </summary>
    public class ServerModel
    {

        /// <summary>
        /// Gets or sets a value indicating whether the proxy server automatically starts with the service.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server automatically starts with the service; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        ///     Gets or sets the fallback response for a request when a remote host is not available and there is no Stump to serve it.
        /// </summary>
        /// <value>
        ///     The fallback response for a request when a remote host is not available and there is no Stump to serve it.
        /// </value>
        public FallbackResponse FallbackResponse { get; set; }

        /// <summary>
        ///     Gets or sets the port that is used to listen for traffic.
        /// </summary>
        /// <value>
        ///     The port used to listen for traffic.
        /// </value>
        public int ListeningPort { get; set; }

        /// <summary>
        ///     Gets or sets the host name for the remote server.
        /// </summary>
        /// <value>
        ///     The host name for the remote server.
        /// </value>
        public string RemoteServerHostName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote HTTP server requires SSL.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the remote HTTP server requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool RemoteServerRequiresSsl { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote HTTP server should be contacted for all unhandled requests.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the remote HTTP server should be contacted for all unhandled HTTP requests; otherwise, <c>false</c>.
        /// </value>
        public bool UseRemoteServerForRequests { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether Stumps when handling requests.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use Stumps when handling requests; otherwise, <c>false</c>.
        /// </value>
        public bool UseStumpsForRequests { get; set; }

    }

}
