namespace Stumps.Server.Data
{
    /// <summary>
    ///     A class that represents the persisted form of an HTTP response.
    /// </summary>
    public class HttpResponseEntity : HttpContextPartEntity
    {
        /// <summary>
        ///     Gets or sets the redirect address.
        /// </summary>
        /// <value>
        ///     The redirect address.
        /// </value>
        public string RedirectAddress
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>
        ///     A value of <c>0</c> or less will not cause a delay.
        /// </remarks>
        public int ResponseDelay
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        public int StatusCode
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        public bool TerminateConnection
        {
            get;
            set;
        }
    }
}
