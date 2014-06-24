namespace Stumps.Server.Data
{

    using System;
    using System.IO;
    using System.Net;

    /// <summary>
    ///     A class that provides an <see cref="T:Stumps.IStumpsHttpRequest"/> implementation using a
    ///     <see cref="T:Stumps.Server.Data.HttpRequestEntity"/> object.
    /// </summary>
    public class HttpRequestEntityReader : IStumpsHttpRequest
    {

        private readonly HttpRequestEntity _entity;
        private readonly byte[] _body;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Data.HttpRequestEntityReader"/> class.
        /// </summary>
        /// <param name="serverId">The unique identifier for the server.</param>
        /// <param name="requestEntity">The request entity.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
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
                throw new ArgumentNullException("serverId");
            }

            if (requestEntity == null)
            {
                throw new ArgumentNullException("requestEntity");

            }

            if (dataAccess == null)
            {
                throw new ArgumentNullException("dataAccess");
            }

            _entity = requestEntity;

            this.Headers = new HttpHeaders();
            foreach (var pair in _entity.Headers)
            {
                this.Headers[pair.Name] = pair.Value;
            }

            _body = dataAccess.ProxyServerReadResource(serverId, requestEntity.BodyFileName) ?? new byte[0];

        }

        /// <summary>
        ///     Gets the length of the HTTP body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP body.
        /// </value>
        public int BodyLength
        {
            get { return _body.Length; }
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
            get { return _entity.HttpMethod; }
        }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        public IPEndPoint LocalEndPoint
        {
            get { return new IPEndPoint(0, 0); }
        }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        public string ProtocolVersion
        {
            get { return _entity.ProtocolVersion; }
        }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        public string RawUrl
        {
            get { return _entity.RawUrl; }
        }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        public IPEndPoint RemoteEndPoint
        {
            get { return new IPEndPoint(0, 0); }
        }

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="T:System.Byte" /> values representing the HTTP body.
        /// </returns>
        public byte[] GetBody()
        {
            return _body;
        }

    }

}
