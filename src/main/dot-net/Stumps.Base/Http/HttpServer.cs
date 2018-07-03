namespace Stumps.Http
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     A class that represents a basic HTTP server.
    /// </summary>
    internal sealed class HttpServer : IHttpServer
    {
        private readonly IHttpHandler _handler;
        private readonly int _port;
        private readonly ServerScheme _scheme;
        private HttpListener _listener;
        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.HttpServer"/> class.
        /// </summary>
        /// <param name="scheme">The transport server scheme used.</param>
        /// <param name="port">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="handler">The default <see cref="T:Stumps.Http.IHttpHandler"/> executed when receiving traffic.</param>
        /// <exception cref="ArgumentNullException">handler</exception>
        /// <exception cref="ArgumentOutOfRangeException">port</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="handler" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        public HttpServer(ServerScheme scheme, int port, IHttpHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            _listener = new HttpListener();
            _port = port;
            _scheme = scheme;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.HttpServer"/> class.
        /// </summary>
        /// <param name="port">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="handler">The default <see cref="T:Stumps.Http.IHttpHandler"/> executed when receiving traffic.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        public HttpServer(int port, IHttpHandler handler) : this(ServerScheme.Http, port, handler)
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:Stumps.Http.HttpServer"/> class.
        /// </summary>
        ~HttpServer() => Dispose();

        /// <summary>
        ///     Occurs when the server processed an incoming HTTP request and returned the response to the client.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> RequestFinished;

        /// <summary>
        ///     Occurs after the server has finished processing the HTTP request, 
        ///     and has constructed a response, but before it returned to the client.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> RequestProcessed;

        /// <summary>
        ///     Occurs when the server receives an incoming HTTP request.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> RequestReceived;

        /// <summary>
        ///     Gets TCP port used by the instance to listen for HTTP requests.
        /// </summary>
        /// <value>
        ///     The port used to listen for HTTP requets.
        /// </value>
        public int Port
        {
            get => _port;
        }

        /// <summary>
        ///     Gets the scheme used to listen for connections by the instance.
        /// </summary>
        /// <value>
        ///     The scheme used to listen for connections by the instance.
        /// </value>
        public ServerScheme Scheme
        {
            get => _scheme;
        }

        /// <summary>
        ///     Gets a value indicating whether the instance is started.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance is started; otherwise, <c>false</c>.
        /// </value>
        public bool Started
        {
            get => _started;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_listener.IsListening)
            {
                _listener.Stop();
            }

            if (_listener is IDisposable disposable)
            {
                disposable.Dispose();
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Starts the instance listening for HTTP requests.
        /// </summary>
        public void StartListening()
        {
            if (_started)
            {
                return;
            }

            _started = true;

            _listener = new HttpListener();

            var httpPattern = this.Scheme == ServerScheme.Https
                                  ? BaseResources.HttpsServerPattern
                                  : BaseResources.HttpServerPattern;

            var url = string.Format(CultureInfo.InvariantCulture, httpPattern, _port);

            _listener.Prefixes.Add(url);
            _listener.Start();
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            WaitForConnections();
        }

        /// <summary>
        ///     Stops the instance from listening for HTTP requests.
        /// </summary>
        public void StopListening()
        {
            try
            {
                _started = false;
                _listener.Stop();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        /// <summary>
        ///     Processes the incoming HTTP request asynchronously.
        /// </summary>
        private async Task ProcessAsyncRequest()
        {
            if (_listener == null || !_listener.IsListening)
            {
                return;
            }

            try
            {
                // Gets the HTTP context for the request
                var context = await _listener.GetContextAsync();
                WaitForConnections();

                // Create a new StumpsHttpContext
                var stumpsContext = new StumpsHttpContext(context);

                this.RequestReceived?.Invoke(this, new StumpsContextEventArgs(stumpsContext));

                // Process the request through the HTTP handler
                var processResult = await _handler.ProcessRequest(stumpsContext);

                this.RequestProcessed?.Invoke(this, new StumpsContextEventArgs(stumpsContext));

                var abortConnection = processResult == ProcessHandlerResult.DropConnection;

                // End the request
                await stumpsContext.EndResponse(abortConnection);

                this.RequestFinished?.Invoke(this, new StumpsContextEventArgs(stumpsContext));
            }
            catch (AggregateException aex)
            {
                aex.Handle((iex) =>
                {
                    if (iex is HttpListenerException || iex is InvalidOperationException)
                    {
                        return true;
                    }

                    return false;
                });
            }
            catch (HttpListenerException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        ///     Wait for incoming HTTP connections.
        /// </summary>
        private void WaitForConnections()
        {
            if (_started && _listener.IsListening)
            {
                Task.Run(async () => await ProcessAsyncRequest());
            }
        }
    }
}