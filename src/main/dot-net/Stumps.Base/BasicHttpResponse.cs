namespace Stumps
{

    using System;
    using Stumps.Http;

    public class BasicHttpResponse
    {

        private byte[] _bodyBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.BasicHttpResponse"/> class.
        /// </summary>
        public BasicHttpResponse()
        {

            this.Headers = new HeaderDictionary();
            this.StatusCode = HttpStatusCodes.HttpOk;
            this.StatusDescription = HttpStatusCodes.GetStatusDescription(this.StatusCode);
            _bodyBuffer = new byte[0];

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
        ///     Gets the collection of HTTP headers returned with the response.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers returned with the response.
        /// </value>
        public IHeaderDictionary Headers
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
        public string RedirectAddress
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        public int StatusCode
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="bytes">The bytes to append to the body of the response.</param>
        public virtual void AppendToBody(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            var newBodyLength = _bodyBuffer.Length + bytes.Length;
            var newBuffer = new byte[newBodyLength];

            Buffer.BlockCopy(_bodyBuffer, 0, newBuffer, 0, _bodyBuffer.Length);
            Buffer.BlockCopy(bytes, 0, newBuffer, _bodyBuffer.Length, bytes.Length);

            _bodyBuffer = newBuffer;
        }

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        public virtual void ClearBody()
        {
            _bodyBuffer = new byte[0];
        }

        /// <summary>
        ///     Gets the body of the HTTP response.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="T:System.Byte" /> that represent the HTTP response.
        /// </returns>
        public virtual byte[] GetBody()
        {
            return _bodyBuffer;
        }

    }

}
