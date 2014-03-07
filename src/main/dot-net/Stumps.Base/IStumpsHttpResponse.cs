namespace Stumps
{

    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    ///     An interface that represents an HTTP response.
    /// </summary>
    public interface IStumpsHttpResponse : IDisposable
    {

        /// <summary>
        ///     Gets or sets the MIME content type of the response.
        /// </summary>
        /// <value>
        ///     The MIME content type of the response.
        /// </value>
        string ContentType { get; set; }

        /// <summary>
        ///     Gets the collection of HTTP headers returned with the response.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers returned with the response.
        /// </value>
        WebHeaderCollection Headers { get; }

        /// <summary>
        ///     Gets the <see cref="T:System.IO.Stream"/> containing the body of the response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.IO.Stream"/> containing the body of the response.
        /// </value>
        Stream OutputStream { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether to send the response using HTTP chunked mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> to send the response using HTTP chunked mode; otherwise, <c>false</c>.
        /// </value>
        bool SendChunked { get; set; }

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
        ///     Adds the HTTP header to the collection.
        /// </summary>
        /// <param name="name">The name of the HTTP header.</param>
        /// <param name="value">The value of the HTTP header.</param>
        void AddHeader(string name, string value);

        /// <summary>
        ///     Clears all data in the <see cref="P:Stumps.IStumpsHttpResponse.OutputStream"/>.
        /// </summary>
        void ClearOutputStream();

        /// <summary>
        ///     Flushes the entire response to the network buffer.
        /// </summary>
        void FlushResponse();

        /// <summary>
        ///     Redirects the client to a new URL.
        /// </summary>
        /// <param name="url">The URL to redirect the client to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#", Justification = "Based on the underlying framework")]
        void Redirect(string url);

    }

}