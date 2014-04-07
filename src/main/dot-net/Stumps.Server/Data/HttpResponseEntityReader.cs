namespace Stumps.Server.Data
{

    using System;
    using System.IO;

    /// <summary>
    ///     A class that provides an <see cref="T:Stumps.IStumpsHttpResponse"/> implementation using a
    ///     <see cref="T:Stumps.Server.Data.HttpResponseEntity"/> object.
    /// </summary>
    public class HttpResponseEntityReader : IStumpsHttpResponse
    {

        private readonly HttpResponseEntity _entity;
        private readonly byte[] _body;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Data.HttpResponseEntityReader"/> class.
        /// </summary>
        /// <param name="responseEntity">The response entity.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="responseEntity"/> is <c>null</c>.</exception>
        public HttpResponseEntityReader(HttpResponseEntity responseEntity)
        {

            if (responseEntity == null)
            {
                throw new ArgumentNullException("responseEntity");

            }

            _entity = responseEntity;

            this.Headers = new HttpHeaders();
            foreach (var pair in _entity.Headers)
            {
                this.Headers[pair.Name] = pair.Value;
            }

            _body = File.Exists(_entity.BodyFileName)
                        ? File.ReadAllBytes(_entity.BodyFileName)
                        : new byte[0];

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
        ///     Gets or sets the redirect address.
        /// </summary>
        /// <value>
        ///     The redirect address.
        /// </value>
        /// <exception cref="System.NotSupportedException">Thrown when altering the value of the redirect address.</exception>
        public string RedirectAddress
        {
            get { return _entity.RedirectAddress; } 
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        /// <exception cref="System.NotSupportedException">Thrown when altering the value of the status code.</exception>
        public int StatusCode
        {
            get { return _entity.StatusCode; } 
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        /// <exception cref="System.NotSupportedException">Thrown when altering the value of the status description.</exception>
        public string StatusDescription
        {
            get { return _entity.StatusDescription; } 
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        /// <exception cref="System.NotSupportedException">Thrown when altering the HTTP body.</exception>
        public void AppendToBody(byte[] buffer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Thrown when altering the HTTP body.</exception>
        public void ClearBody()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="T:System.Byte"/> values representing the HTTP body.
        /// </returns>
        public byte[] GetBody()
        {
            return _body;
        }

    }

}
