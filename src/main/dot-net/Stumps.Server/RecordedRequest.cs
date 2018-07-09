namespace Stumps.Server
{
    using System;
    using System.Net;

    /// <summary>
    ///     A class that represents a recorded HTTP request.
    /// </summary>
    public sealed class RecordedRequest : RecordedContextPartBase, IStumpsHttpRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordedRequest" /> class.
        /// </summary>
        /// <param name="request">The <see cref="IStumpsHttpRequest"/> used to initialize the instance.</param>
        /// <param name="decoderHandling">The <see cref="ContentDecoderHandling"/> requirements for the HTTP body.</param>
        /// <exception cref="ArgumentNullException"><paramref name="request"/> is <c>null</c>.</exception>
        public RecordedRequest(IStumpsHttpRequest request, ContentDecoderHandling decoderHandling)
            : base(request, decoderHandling)
        {
            request = request ?? throw new ArgumentNullException(nameof(request));

            this.HttpMethod = request.HttpMethod;
            this.LocalEndPoint = request.LocalEndPoint ?? new IPEndPoint(0, 0);
            this.ProtocolVersion = request.ProtocolVersion;
            this.RawUrl = request.RawUrl;
            this.RemoteEndPoint = request.RemoteEndPoint ?? new IPEndPoint(0, 0);
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
        }
    }
}