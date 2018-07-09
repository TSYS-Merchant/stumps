namespace Stumps.Server.Data
{
    using System;

    /// <summary>
    ///     A class that provides an <see cref="IStumpsHttpResponse"/> implementation using a
    ///     <see cref="HttpResponseEntity"/> object.
    /// </summary>
    public class HttpResponseEntityReader : IStumpsHttpResponse
    {
        private readonly HttpResponseEntity _entity;
        private readonly byte[] _body;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpResponseEntityReader"/> class.
        /// </summary>
        /// <param name="serverId">The unique identifier for the server.</param>
        /// <param name="responseEntity">The response entity.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serverId"/> is <c>null</c>.
        /// or
        /// <paramref name="responseEntity"/> is <c>null</c>.
        /// or 
        /// <paramref name="dataAccess"/> is <c>null</c>.
        /// </exception>
        public HttpResponseEntityReader(string serverId, HttpResponseEntity responseEntity, IDataAccess dataAccess)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));

            _entity = responseEntity ?? throw new ArgumentNullException(nameof(responseEntity));

            this.Headers = new HttpHeaders();
            foreach (var pair in _entity.Headers)
            {
                this.Headers[pair.Name] = pair.Value;
            }

            _body = dataAccess.ServerReadResource(serverId, responseEntity.BodyResourceName) ?? new byte[0];
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
            private set;
        }

        /// <summary>
        ///     Gets or sets the redirect address.
        /// </summary>
        /// <value>
        ///     The redirect address.
        /// </value>
        /// <exception cref="NotSupportedException">Thrown when altering the value of the redirect address.</exception>
        public string RedirectAddress
        {
            get => _entity.RedirectAddress;
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>
        ///     A value of <c>0</c> or less will not cause a delay.
        /// </remarks>
        /// <exception cref="NotSupportedException">Thrown when altering the value of the response delay.</exception>
        public int ResponseDelay
        {
            get => _entity.ResponseDelay;
            set => throw new NotImplementedException();
        }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        /// <exception cref="NotSupportedException">Thrown when altering the value of the status code.</exception>
        public int StatusCode
        {
            get => _entity.StatusCode;
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        /// <exception cref="NotSupportedException">Thrown when altering the value of the status description.</exception>
        public string StatusDescription
        {
            get => _entity.StatusDescription;
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        /// <exception cref="NotSupportedException">Thrown when altering the value indicating to terminate the connection.</exception>
        public bool TerminateConnection
        {
            get => _entity.TerminateConnection;
            set => throw new NotImplementedException();
        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        /// <exception cref="NotSupportedException">Thrown when altering the HTTP body.</exception>
        public void AppendToBody(byte[] buffer) => throw new NotSupportedException();

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when altering the HTTP body.</exception>
        public void ClearBody() => throw new NotSupportedException();

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="Byte"/> values representing the HTTP body.
        /// </returns>
        public byte[] GetBody() => _body;
    }
}
