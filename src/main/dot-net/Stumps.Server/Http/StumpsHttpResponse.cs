namespace Stumps.Http
{

    using System;
    using System.IO;
    using System.Net;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents the response to an HTTP request.
    /// </summary>
    internal sealed class StumpsHttpResponse : IStumpsHttpResponse
    {

        private readonly HttpListenerResponse _response;
        private bool _disposed;
        private MemoryStream _responseStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.Http.StumpsHttpResponse"/> class.
        /// </summary>
        /// <param name="response">The response.</param>
        public StumpsHttpResponse(HttpListenerResponse response)
        {
            _response = response;
            _responseStream = new MemoryStream();

            this.StatusCode = HttpStatusCodes.HttpOk;
            this.StatusDescription = HttpStatusCodes.GetStatusDescription(this.StatusCode);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:Stumps.Http.StumpsHttpResponse"/> class.
        /// </summary>
        ~StumpsHttpResponse()
        {
            Dispose();
        }

        /// <summary>
        ///     Gets the underlying <see cref="T:System.Net.HttpListenerResponse"/>.
        /// </summary>
        /// <value>
        ///     The underlying <see cref="T:System.Net.HttpListenerResponse"/>.
        /// </value>
        public HttpListenerResponse ListenerResponse
        {
            get { return _response; }
        }

        /// <summary>
        ///     Gets or sets the MIME content type of the response.
        /// </summary>
        /// <value>
        ///     The MIME content type of the response.
        /// </value>
        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        /// <summary>
        ///     Gets the collection of HTTP headers returned with the response.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers returned with the response.
        /// </value>
        public WebHeaderCollection Headers
        {
            get { return _response.Headers; }
        }

        /// <summary>
        ///     Gets the <see cref="T:System.IO.Stream" /> containing the body of the response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.IO.Stream" /> containing the body of the response.
        /// </value>
        public Stream OutputStream
        {
            get { return _responseStream; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to send the response using HTTP chunked mode.
        /// </summary>
        /// <value>
        ///     <c>true</c> to send the response using HTTP chunked mode; otherwise, <c>false</c>.
        /// </value>
        public bool SendChunked { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        public string StatusDescription { get; set; }

        /// <summary>
        ///     Adds the HTTP header to the collection.
        /// </summary>
        /// <param name="name">The name of the HTTP header.</param>
        /// <param name="value">The value of the HTTP header.</param>
        public void AddHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        /// <summary>
        ///     Clears all data in the <see cref="P:Stumps.Http.IStumpsHttpResponse.OutputStream" />.
        /// </summary>
        public void ClearOutputStream()
        {
            _responseStream.Close();
            _responseStream = new MemoryStream();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (_responseStream != null && !_disposed)
            {
                _responseStream.Dispose();
                _responseStream = null;
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Flushes the entire response to the network buffer.
        /// </summary>
        public void FlushResponse()
        {

            // Dump out the response output stream
            StreamUtility.CopyStream(_responseStream, _response.OutputStream, 0);

            // Complete and close the request
            _responseStream.Close();
            _response.OutputStream.Close();
            _response.Close();

        }

        /// <summary>
        ///     Redirects the client to a new URL.
        /// </summary>
        /// <param name="url">The URL to redirect the client to.</param>
        public void Redirect(string url)
        {
            _response.Redirect(url);
        }

    }

}