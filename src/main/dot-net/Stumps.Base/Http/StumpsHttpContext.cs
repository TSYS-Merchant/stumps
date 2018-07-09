namespace Stumps.Http
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     A class that represents the complete context for an HTTP request.
    /// </summary>
    internal sealed class StumpsHttpContext : IStumpsHttpContext
    {

        private HttpListenerContext _context;
        private StumpsHttpRequest _request;
        private StumpsHttpResponse _response;
        private int initializeCalled = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsHttpContext"/> class.
        /// </summary>
        public StumpsHttpContext()
        {
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
        ///     Gets the <see cref="IStumpsHttpRequest" /> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="IStumpsHttpRequest" /> object for the current HTTP request.
        /// </value>
        public IStumpsHttpRequest Request
        {
            get => _request;
        }

        /// <summary>
        ///     Gets the <see cref="IStumpsHttpResponse" /> object for the current HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="IStumpsHttpResponse" /> object for the current HTTP response.
        /// </value>
        public IStumpsHttpResponse Response
        {
            get => _response;
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
        /// <param name="abort">if set to <c>true</c>, the connection is aborted without responding.</param>
        public async Task EndResponse(bool abort)
        {
            // Forceably abort the connection
            if (abort)
            {
                _context.Response.Abort();
                return;
            }

            // Set the status codes
            _context.Response.StatusCode = _response.StatusCode;
            _context.Response.StatusDescription = _response.StatusDescription;

            // Write headers
            WriteHeaders();

            // Write the body
            await WriteBody();

            _context.Response.Close();
        }

        /// <summary>
        ///     Initializes the instance with a specified <see cref="HttpListenerContext"/> object.
        /// </summary>
        /// <param name="context">The <see cref="HttpListenerContext"/> used to initialize the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task InitializeInstance(HttpListenerContext context)
        {
            var methodAlreadyCalled = Interlocked.CompareExchange(ref initializeCalled, 1, 0) == 1;

            if (methodAlreadyCalled)
            {
                throw new InvalidOperationException("The object was already initialized with an existing context.");
            }

            context = context ?? throw new ArgumentNullException(nameof(context));

            this.UniqueIdentifier = Guid.NewGuid();
            this.ReceivedDate = DateTime.Now;

            _context = context;

            // Initialize the HTTP request for the context
            _request = new StumpsHttpRequest();

            await _request.InitializeInstance(context.Request);
        }

        /// <summary>
        ///     Writes the body to the HTTP listener response.
        /// </summary>
        private async Task WriteBody()
        {
            if (_response.BodyLength > 0)
            {
                await _context.Response.OutputStream.WriteAsync(_response.GetBody(), 0, _response.BodyLength);
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

            // content length
            _context.Response.ContentLength64 = _response.BodyLength;

            // Add all headers
            foreach (var headerName in _response.Headers.HeaderNames)
            {
                if (IgnoredHeaders.IsIgnored(headerName))
                {
                    continue;
                }

                try
                {
                    var writeName = headerName;
                    var writeValue = _response.Headers[headerName];

                    if (HttpHeaderSanitization.SanitizeHeader(ref writeName, ref writeValue))
                    {
                        _context.Response.Headers.Add(writeName, writeValue);
                    }
                }
                catch (ArgumentException)
                {
                    // The header could fail to add because it is being referenced
                    // as a property - this is OK.
                    // TODO: Log error
                }
            }
        }
    }
}