namespace Stumps.Web.Api
{

    /// <summary>
    ///     A class that represents the model for the status response for a Stumps server.
    /// </summary>
    public class ServerStatusModel
    {

        /// <summary>
        ///     Gets or sets a value indicating whether the server is recording requests.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the server is requests; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecording { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the server is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the server is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning { get; set; }

        /// <summary>
        ///     Gets or sets the total number of recorded requests.
        /// </summary>
        /// <value>
        ///     The total number of recorded requests.
        /// </value>
        public int RecordedRequests { get; set; }

        /// <summary>
        ///     Gets or sets the total number of requests handled by the server.
        /// </summary>
        /// <value>
        ///     The total number of requests handled by the server.
        /// </value>
        public int RequestsHandledByServer { get; set; }

        /// <summary>
        ///     Gets or sets the total number of requests handled by the remote server.
        /// </summary>
        /// <value>
        ///     The total number of requests handled by the remote server.
        /// </value>
        public int RequestsHandledByRemoteServer { get; set; }

        /// <summary>
        ///     Gets or sets the total number of requests handled a stump.
        /// </summary>
        /// <value>
        ///     The total number of requests handled by a stump.
        /// </value>
        public int RequestsHandledByStump { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId { get; set; }

        /// <summary>
        ///     Gets or sets the total number of stumps in the server.
        /// </summary>
        /// <value>
        ///     The total number of stumps in server.
        /// </value>
        public int StumpsInServer { get; set; }

    }

}
