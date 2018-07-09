namespace Stumps
{
    /// <summary>
    ///     An interface that represents an HTTP response.
    /// </summary>
    public interface IStumpsHttpResponse : IStumpsHttpContextPart
    {
        /// <summary>
        ///     Gets or sets the redirect address.
        /// </summary>
        /// <value>
        ///     The redirect address.
        /// </value>
        string RedirectAddress { get; set; }

        /// <summary>
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>A value of <c>0</c> or less will not cause a delay.</remarks>
        int ResponseDelay { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        string StatusDescription { get; set; }

        /// <summary>
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        bool TerminateConnection { get; set; }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        void AppendToBody(byte[] buffer);

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        void ClearBody();
    }
}
