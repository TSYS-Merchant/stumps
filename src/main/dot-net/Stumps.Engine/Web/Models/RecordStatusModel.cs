namespace Stumps.Web.Models
{

    /// <summary>
    ///     A class that represents the recording status of a proxy server.
    /// </summary>
    public class RecordStatusModel
    {

        /// <summary>
        ///     Gets or sets a value indicating whether the proxy server is recording traffic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the proxy server is recording traffic; otherwise, <c>false</c>.
        /// </value>
        public bool RecordTraffic { get; set; }

    }

}