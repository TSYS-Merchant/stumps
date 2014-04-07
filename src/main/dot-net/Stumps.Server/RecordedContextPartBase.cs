namespace Stumps.Server
{

    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    ///     A class that provides the foundation for recorded HTTP requests and responses. 
    /// </summary>
    public abstract class RecordedContextPartBase : IStumpsHttpContextPart
    {

        private byte[] _bodyBuffer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RecordedContextPartBase" /> class.
        /// </summary>
        /// <param name="contextPart">The context part.</param>
        /// <param name="decoderHandling">The <see cref="T:Stumps.Server.ContentDecoderHandling"/> requirements for the HTTP body.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="contextPart"/> is <c>null</c>.</exception>
        protected RecordedContextPartBase(IStumpsHttpContextPart contextPart, ContentDecoderHandling decoderHandling)
        {

            if (contextPart == null)
            {
                throw new ArgumentNullException("contextPart");
            }

            // Copy in the headers
            this.Headers = new HttpHeaders();
            contextPart.Headers.CopyTo(this.Headers);

            // Copy the body into the context part
            _bodyBuffer = new byte[contextPart.BodyLength];

            if (_bodyBuffer.Length > 0)
            {
                Buffer.BlockCopy(contextPart.GetBody(), 0, _bodyBuffer, 0, _bodyBuffer.Length);
            }

            // Decode the body if necessary
            if (decoderHandling == ContentDecoderHandling.DecodeRequired)
            {
                DecodeBody();
            }

            this.ExamineBody();

        }

        /// <summary>
        ///     Gets the length of the HTTP body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP body.
        /// </value>
        public int BodyLength
        {
            get { return _bodyBuffer.Length; }
        }

        /// <summary>
        ///     Gets the MD5 hash of the HTTP body.
        /// </summary>
        /// <value>
        ///     The MD5 hash of the HTTP body.
        /// </value>
        public string BodyMd5Hash
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        /// The collection of HTTP headers.
        /// </value>
        public IHttpHeaders Headers
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets information about how the HTTP body was classified.
        /// </summary>
        /// <value>
        ///     The information about how the HTTP body was classified.
        /// </value>
        public HttpBodyClassification BodyType
        {
            get; 
            private set;
        }
        
        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="T:System.Byte"/> values representing the HTTP body.
        /// </returns>
        public byte[] GetBody()
        {
            return _bodyBuffer;
        }

        /// <summary>
        ///     Gets the HTTP body as a <see cref="T:System.String"/>.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> representing the body of the HTTP response.</returns>
        /// <remarks>The body is decoded using UTF8 encoding.</remarks>
        public string GetBodyAsString()
        {
            return GetBodyAsString(Encoding.UTF8);
        }

        /// <summary>
        ///     Gets the HTTP body as a <see cref="T:System.String"/>.
        /// </summary>
        /// <param name="encoding">The encoding used to convert the HTTP body into a <see cref="T:System.String"/>.</param>
        /// <remarks>The body is decoded using UTF8 encoding.</remarks>
        public string GetBodyAsString(Encoding encoding)
        {

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            return encoding.GetString(_bodyBuffer);

        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        protected void AppendToBody(byte[] buffer)
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
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        protected void ClearBody()
        {
            _bodyBuffer = new byte[0];
        }

        /// <summary>
        ///     Examines the body for the classification, and the MD5 hash.
        /// </summary>
        protected void ExamineBody()
        {

            // Determine the body type
            DetermineBodyClassification();

            // Generate the MD5 hash of the body
            GenerateMd5Hash();

        }

        /// <summary>
        ///     Decodes the body of a based on the content encoding.
        /// </summary>
        private void DecodeBody()
        {

            var contentEncoding = this.Headers["Content-Encoding"];

            if (contentEncoding != null)
            {
                var encoder = new ContentEncoder(contentEncoding);
                _bodyBuffer = encoder.Decode(_bodyBuffer);
            }

        }

        /// <summary>
        ///     Determines the HTTP body to determine its classification.
        /// </summary>
        private void DetermineBodyClassification()
        {
            
            if (_bodyBuffer.Length == 0)
            {
                this.BodyType = HttpBodyClassification.Empty;
            }
            else if (this.Headers["Content-Type"].StartsWith("image", StringComparison.Ordinal))
            {
                this.BodyType = HttpBodyClassification.Image;
            }
            else if (TextAnalyzer.IsText(_bodyBuffer))
            {
                this.BodyType = HttpBodyClassification.Text;
            }
            else
            {
                this.BodyType = HttpBodyClassification.Binary;
            }

        }

        /// <summary>
        /// Generates the MD5 hash for the HTTP body.
        /// </summary>
        private void GenerateMd5Hash()
        {
        
            if (_bodyBuffer == null || _bodyBuffer.Length == 0)
            {
                this.BodyMd5Hash = string.Empty;
                return;
            }

            using (var hash = MD5.Create())
            {
                var bytes = hash.ComputeHash(_bodyBuffer);
                this.BodyMd5Hash = bytes.ToHexString();
            }

        }

    }

}
