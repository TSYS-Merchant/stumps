namespace Stumps
{

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;

    /// <summary>
    ///     An interface that represents an incomming HTTP request.
    /// </summary>
    public interface IStumpsHttpRequest : IDisposable
    {

        /// <summary>
        ///     Gets the MIME content type of the request.
        /// </summary>
        /// <value>
        ///     The MIME content type of the request.
        /// </value>
        string ContentType { get; }

        /// <summary>
        ///     Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        IHeaderDictionary Headers { get; }

        /// <summary>
        ///     Gets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>
        ///     The HTTP data transfer method used by the client.
        /// </value>
        string HttpMethod { get; }

        /// <summary>
        ///     Gets the <see cref="T:System.IO.Stream"/> containing the HTTP request body.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.IO.Stream"/> containing the HTTP request body.
        /// </value>
        Stream InputStream { get; }

        /// <summary>
        ///     Gets a value indicating whether the connection is using a secure channel.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection is using a secure channel; otherwise, <c>false</c>.
        /// </value>
        bool IsSecureConnection { get; }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        Version ProtocolVersion { get; }

        /// <summary>
        ///     Gets the collection of HTTP query string variables.
        /// </summary>
        /// <value>
        ///     The collection of HTTP query string variables.
        /// </value>
        NameValueCollection QueryString { get; }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        string RawUrl { get; }

        /// <summary>
        ///     Gets the URL for the client's previous request that linked to the current URL.
        /// </summary>
        /// <value>
        ///     The URL for the client's previous request that linked to the current URL.
        /// </value>
        string Referer { get; }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        ///     Gets the URL for the current request.
        /// </summary>
        /// <value>
        ///     The URL for the current request.
        /// </value>
        Uri Url { get; }

        /// <summary>
        ///     Gets user agent for the client's browser.
        /// </summary>
        /// <value>
        ///     The user agent for the client's browser.
        /// </value>
        string UserAgent { get; }

    }

}