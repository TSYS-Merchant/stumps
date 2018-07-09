namespace Stumps.Server.Data
{
    using System;
    using System.Net;

    /// <summary>
    ///     A class that provides an <see cref="IStumpsHttpRequest"/> implementation using a
    ///     <see cref="HttpRequestEntity"/> object.
    /// </summary>
    public class HttpRequestEntityReader : IStumpsHttpRequest
    {
        private readonly HttpRequestEntity _entity;
        private readonly byte[] _body;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpRequestEntityReader"/> class.
        /// </summary>
        /// <param name="serverId">The unique identifier for the server.</param>
        /// <param name="requestEntity">The request entity.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serverId"/> is <c>null</c>.
        /// or
        /// <paramref name="requestEntity"/> is <c>null</c>.
        /// or 
        /// <paramref name="dataAccess"/> is <c>null</c>.
        /// </exception>
        public HttpRequestEntityReader(string serverId, HttpRequestEntity requestEntity, IDataAccess dataAccess)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));

            _entity = requestEntity ?? throw new ArgumentNullException(nameof(requestEntity));

            foreach (var pair in _entity.Headers)
            {
                this.Headers[pair.Name] = pair.Value;
            }

            _body = dataAccess.ServerReadResource(serverId, requestEntity.BodyResourceName) ?? new byte[0];
        }

        /// <summary>
        ///     Gets the length of the HTTP body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP body.
        /// </value>
        public int BodyLength
        {
            get => _body.Length;
        }

        /// <summary>
        ///     Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        public IHttpHeaders Headers
        {
            get;
        } = new HttpHeaders();

        /// <summary>
        ///     Gets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>
        ///     The HTTP data transfer method used by the client.
        /// </value>
        public string HttpMethod
        {
            get => _entity.HttpMethod;
        }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        public IPEndPoint LocalEndPoint
        {
            get => new IPEndPoint(0, 0);
        }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        public string ProtocolVersion
        {
            get => _entity.ProtocolVersion;
        }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        public string RawUrl
        {
            get => _entity.RawUrl;
        }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        public IPEndPoint RemoteEndPoint
        {
            get => new IPEndPoint(0, 0);
        }

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="Byte" /> values representing the HTTP body.
        /// </returns>
        public byte[] GetBody() => _body;
    }
}
