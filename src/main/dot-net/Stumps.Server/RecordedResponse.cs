namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     A class that represents a recorded HTTP response.
    /// </summary>
    public sealed class RecordedResponse : RecordedContextPartBase, IStumpsHttpResponse
    {

        private readonly string _redirectAddress;
        private readonly int _statusCode;
        private readonly string _statusDescription;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RecordedResponse" /> class.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.IStumpsHttpResponse"/> used to initialize the instance.</param>
        public RecordedResponse(IStumpsHttpResponse response) : base(response)
        {
            _redirectAddress = response.RedirectAddress;
            _statusCode = response.StatusCode;
            _statusDescription = response.StatusDescription;
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
            get { return _redirectAddress; }
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
            get { return _statusCode; }
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
            get { return _statusDescription; }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        /// <exception cref="System.NotSupportedException">Always thrown.</exception>
        public void AppendToBody(byte[] buffer)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Always thrown.</exception>
        public void ClearBody()
        {
            throw new NotSupportedException();
        }

    }

}