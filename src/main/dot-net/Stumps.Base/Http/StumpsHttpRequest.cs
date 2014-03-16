namespace Stumps.Http
{

    using System;
    using System.Net;
    using Stumps.Utility;
    using System.Globalization;

    /// <summary>
    ///     A class that represents an incomming HTTP request.
    /// </summary>
    internal sealed class StumpsHttpRequest : IStumpsHttpRequest
    {

        private byte[] _bodyBuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.StumpsHttpRequest"/> class.
        /// </summary>
        public StumpsHttpRequest()
        {
            _bodyBuffer = new byte[0];
            this.Headers = new HeaderDictionary();
        }

        /// <summary>
        ///     Gets the length of the HTTP request body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP request body.
        /// </value>
        public int BodyLength
        {
            get { return _bodyBuffer.Length; }
        }

        /// <summary>
        ///     Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        public IHeaderDictionary Headers
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>
        ///     The HTTP data transfer method used by the client.
        /// </value>
        public string HttpMethod
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        public IPEndPoint LocalEndPoint
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        public string ProtocolVersion
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        public string RawUrl
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        public IPEndPoint RemoteEndPoint
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the body for the HTTP request.
        /// </summary>
        /// <returns>
        ///     The body for the HTTP request
        /// </returns>
        public byte[] GetBody()
        {
            return _bodyBuffer;
        }

        /// <summary>
        ///     Initializes the instance using the specified <see cref="T:System.Net.HttpListenerRequest"/>.
        /// </summary>
        /// <param name="request">The <see cref="T:System.Net.HttpListenerRequest"/> used to initilize the instance.</param>
        public void InitializeInstance(HttpListenerRequest request)
        {

            // Setup the standard values
            this.HttpMethod = request.HttpMethod;
            this.LocalEndPoint = request.LocalEndPoint;
            this.ProtocolVersion = String.Format(
                CultureInfo.InvariantCulture, "{0}.{1}", request.ProtocolVersion.Major, request.ProtocolVersion.Minor);
            this.RawUrl = request.RawUrl;
            this.RemoteEndPoint = request.RemoteEndPoint;

            // Setup the body
            _bodyBuffer = StreamUtility.ConvertStreamToByteArray(request.InputStream);

            // Setup the headers
            foreach (var key in request.Headers.AllKeys)
            {
                this.Headers.AddOrUpdate(key, request.Headers[key]);
            }

        }

    }

}