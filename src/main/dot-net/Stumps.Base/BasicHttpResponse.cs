namespace Stumps
{
    using System;
    using System.Text;
    using Stumps.Http;

    /// <summary>
    ///     A class representing a simple and basic HTTP response.
    /// </summary>
    public class BasicHttpResponse : IStumpsHttpResponse
    {
        private byte[] _bodyBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.BasicHttpResponse"/> class.
        /// </summary>
        public BasicHttpResponse()
        {
            _bodyBuffer = new byte[0];
        }

        /// <summary>
        ///     Gets the length of the HTTP body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP body.
        /// </value>
        public int BodyLength
        {
            get => _bodyBuffer.Length;
        }

        /// <summary>
        ///     Gets the collection of HTTP headers returned with the response.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers returned with the response.
        /// </value>
        public IHttpHeaders Headers
        {
            get;
        } = new HttpHeaders();

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
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>A value of <c>0</c> or less will not cause a delay.</remarks>
        public int ResponseDelay
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
        } = HttpStatusCodes.HttpOk;

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
        } = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpOk);

        /// <summary>
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        public bool TerminateConnection
        {
            get;
            set;
        }
        
        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        public virtual void AppendToBody(byte[] buffer)
        {
            if (buffer == null)
            {
                return;
            }

            var newBodyLength = _bodyBuffer.Length + buffer.Length;
            var newBuffer = new byte[newBodyLength];

            Buffer.BlockCopy(_bodyBuffer, 0, newBuffer, 0, _bodyBuffer.Length);
            Buffer.BlockCopy(buffer, 0, newBuffer, _bodyBuffer.Length, buffer.Length);

            _bodyBuffer = newBuffer;
        }

        /// <summary>
        ///     Appends a string value to the body of the HTTP response.
        /// </summary>
        /// <param name="value">The value to append to the body of the HTTP response.</param>
        /// <remarks>The <paramref name="value"/> will be converted to a byte array using UTF8 encoding.</remarks>
        public virtual void AppendToBody(string value) => AppendToBody(value, Encoding.UTF8);

        /// <summary>
        /// Appends a string value to the body of the HTTP response using a specified encoding.
        /// </summary>
        /// <param name="value">The value to append to the body of the HTTP response.</param>
        /// <param name="encoding">The encoding used to convert the <paramref name="value"/> into a byte array.</param>
        public virtual void AppendToBody(string value, Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var bytes = encoding.GetBytes(value);

            AppendToBody(bytes);
        }

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        public virtual void ClearBody() => _bodyBuffer = new byte[0];

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="T:System.Byte"/> values representing the HTTP body.
        /// </returns>
        public virtual byte[] GetBody() => _bodyBuffer;

        /// <summary>
        ///     Gets the body of the HTTP response as a <see cref="T:System.String"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> representing the body of the HTTP response.</returns>
        /// <remarks>The body is decoded using UTF8 encoding.</remarks>
        public virtual string GetBodyAsString() => GetBodyAsString(Encoding.UTF8);

        /// <summary>
        ///     Gets the body of the HTTP response as a <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="encoding">The encoding used to convert the HTTP body into a <see cref="T:System.String"/>.</param>
        /// <remarks>The body is decoded using UTF8 encoding.</remarks>
        public virtual string GetBodyAsString(Encoding encoding)
        {
            encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));

            return encoding.GetString(_bodyBuffer);
        }
    }
}
