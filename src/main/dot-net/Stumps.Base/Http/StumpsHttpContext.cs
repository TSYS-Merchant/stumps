namespace Stumps.Http
{

    using System;
    using System.Net;

    /// <summary>
    ///     A class that represents the complete context for an HTTP request.
    /// </summary>
    internal sealed class StumpsHttpContext : IStumpsHttpContext
    {

        private readonly HttpListenerContext _context;
        private readonly StumpsHttpRequest _request;
        private readonly StumpsHttpResponse _response;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.StumpsHttpContext"/> class.
        /// </summary>
        /// <param name="context">The <see cref="T:System.Net.HttpListenerContext"/> used to initialize the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public StumpsHttpContext(HttpListenerContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.UniqueIdentifier = Guid.NewGuid();
            this.ReceivedDate = DateTime.Now;

            _context = context;

            // Initialize the HTTP request for the context
            _request = new StumpsHttpRequest();
            _request.InitializeInstance(context.Request);

            // Initialize the HTTP response for the context
            _response = new StumpsHttpResponse();

        }

        /// <summary>
        /// Gets the received date and time the request was received.
        /// </summary>
        /// <value>
        /// The date and time the request was received.
        /// </value>
        public DateTime ReceivedDate
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpRequest" /> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpRequest" /> object for the current HTTP request.
        /// </value>
        public IStumpsHttpRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse" /> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse" /> object for the current HTTP request.
        /// </value>
        public IStumpsHttpResponse Response
        {
            get { return _response; }
        }

        /// <summary>
        ///     Gets the unique identifier for the HTTP context.
        /// </summary>
        /// <value>
        ///     The unique identifier for the HTTP context.
        /// </value>
        public Guid UniqueIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        ///     Closes the HTTP context and responds to the calling client.
        /// </summary>
        public void EndResponse()
        {

            // Write headers
            WriteHeaders();

            // Write the body
            WriteBody();

            // Set the status codes
            _context.Response.StatusCode = _response.StatusCode;
            _context.Response.StatusDescription = _response.StatusDescription;

            _context.Response.Close();

        }

        /// <summary>
        ///     Writes the body to the HTTP listener response.
        /// </summary>
        private void WriteBody()
        {

            if (_response.BodyLength > 0)
            {
                _context.Response.OutputStream.Write(_response.GetBody(), 0, _response.BodyLength);
            }

        }

        /// <summary>
        ///     Writes the headers to the HTTP listener response.
        /// </summary>
        private void WriteHeaders()
        {

            // content type
            _context.Response.ContentType = _response.Headers["content-type"] ?? string.Empty;

            // chunked
            _context.Response.SendChunked = (_response.Headers["transfer-encoding"] ?? string.Empty).Equals(
                "chunked", StringComparison.OrdinalIgnoreCase);

            // Add all headers
            foreach (var headerName in _response.Headers.HeaderNames)
            {
                if (IgnoredHeaders.IsIgnored(headerName))
                {
                    continue;
                }

                _context.Response.Headers.Add(headerName, _response.Headers[headerName]);
            }

        }

    }

}