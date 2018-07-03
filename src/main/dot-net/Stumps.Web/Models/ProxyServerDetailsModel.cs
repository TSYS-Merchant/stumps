namespace Stumps.Web.Models
{
    /// <summary>
    ///     A class that represents the model for a proxy server that includes detailed information.
    /// </summary>
    public class ProxyServerDetailsModel : ProxyServerModel
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the proxy server is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the proxy server.
        /// </summary>
        /// <value>
        /// The unique identifier for the proxy server.
        /// </value>
        public string ProxyId { get; set; }

        /// <summary>
        ///     Gets or sets the number of HTTP transactions recorded by the proxy server.
        /// </summary>
        /// <value>
        ///     The number of HTTP transactions recorded by the proxy server.
        /// </value>
        public int RecordCount { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the proxy server is recording traffic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server is recording traffic; otherwise, <c>false</c>.
        /// </value>
        public bool RecordTraffic { get; set; }

        /// <summary>
        ///     Gets or sets the total number of HTTP requests served by the proxy server.
        /// </summary>
        /// <value>
        ///     The number of HTTP requests served by the proxy server.
        /// </value>
        public int RequestsServed { get; set; }

        /// <summary>
        ///     Gets or sets the number of Stumps supported by the proxy server.
        /// </summary>
        /// <value>
        /// The number of Stumps supported by the proxy server.
        /// </value>
        public int StumpsCount { get; set; }

        /// <summary>
        ///     Gets or sets the total number of Stumps served by the proxy server.
        /// </summary>
        /// <value>
        ///     The number of Stumps served by the proxy server.
        /// </value>
        public int StumpsServed { get; set; }
    }
}