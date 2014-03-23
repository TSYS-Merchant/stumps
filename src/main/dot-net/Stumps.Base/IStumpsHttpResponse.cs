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
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="bytes">The bytes to append to the body of the response.</param>
        void AppendToBody(byte[] bytes);

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        void ClearBody();

    }

}