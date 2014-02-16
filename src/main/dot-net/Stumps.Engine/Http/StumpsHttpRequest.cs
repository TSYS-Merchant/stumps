namespace Stumps.Http
{

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents an incomming HTTP request.
    /// </summary>
    internal sealed class StumpsHttpRequest : IStumpsHttpRequest
    {

        private readonly HttpListenerRequest _request;
        private bool _disposed;
        private MemoryStream _requestStream;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.StumpsHttpRequest"/> class.
        /// </summary>
        /// <param name="request">The <see cref="T:System.Net.HttpListenerRequest"/> used to initialize the instance.</param>
        public StumpsHttpRequest(HttpListenerRequest request)
        {

            _request = request;

            _requestStream = new MemoryStream();

            StreamUtility.CopyStream(_request.InputStream, _requestStream);

            // Reset the position
            _requestStream.Position = 0;

        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Http.StumpsHttpRequest"/> class.
        /// </summary>
        ~StumpsHttpRequest()
        {
            Dispose();
        }

        /// <summary>
        ///     Gets the MIME content type of the request.
        /// </summary>
        /// <value>
        ///     The MIME content type of the request.
        /// </value>
        public string ContentType
        {
            get { return _request.ContentType; }
        }

        /// <summary>
        ///     Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        public NameValueCollection Headers
        {
            get { return _request.Headers; }
        }

        /// <summary>
        ///     Gets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>
        ///     The HTTP data transfer method used by the client.
        /// </value>
        public string HttpMethod
        {
            get { return _request.HttpMethod; }
        }

        /// <summary>
        ///     Gets the <see cref="T:System.IO.Stream" /> containing the HTTP request body.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.IO.Stream" /> containing the HTTP request body.
        /// </value>
        public Stream InputStream
        {
            get { return _requestStream; }
        }

        /// <summary>
        ///     Gets a value indicating whether the connection is using a secure channel.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the connection is using a secure channel; otherwise, <c>false</c>.
        /// </value>
        public bool IsSecureConnection
        {
            get { return _request.IsSecureConnection; }
        }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        public IPEndPoint LocalEndPoint
        {
            get { return _request.LocalEndPoint; }
        }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        public Version ProtocolVersion
        {
            get { return _request.ProtocolVersion; }
        }

        /// <summary>
        ///     Gets the collection of HTTP query string variables.
        /// </summary>
        /// <value>
        ///     The collection of HTTP query string variables.
        /// </value>
        public NameValueCollection QueryString
        {
            get { return _request.QueryString; }
        }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        public string RawUrl
        {
            get { return _request.RawUrl; }
        }

        /// <summary>
        ///     Gets the URL for the client's previous request that linked to the current URL.
        /// </summary>
        /// <value>
        ///     The URL for the client's previous request that linked to the current URL.
        /// </value>
        public string Referer
        {
            get
            {
                string referer = null;

                if (_request.UrlReferrer != null)
                {
                    referer = _request.UrlReferrer.ToString();
                }

                return referer;
            }
        }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        public IPEndPoint RemoteEndPoint
        {
            get { return _request.RemoteEndPoint; }
        }

        /// <summary>
        ///     Gets the URL for the current request.
        /// </summary>
        /// <value>
        ///     The URL for the current request.
        /// </value>
        public Uri Url
        {
            get { return _request.Url; }
        }

        /// <summary>
        ///     Gets user agent for the client's browser.
        /// </summary>
        /// <value>
        ///     The user agent for the client's browser.
        /// </value>
        public string UserAgent
        {
            get { return _request.UserAgent; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (_requestStream != null && !_disposed)
            {
                _requestStream.Dispose();
                _requestStream = null;
            }

            _disposed = true;

            GC.SuppressFinalize(this);

        }

    }

}