namespace Stumps.Http
{

    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
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
        /// <param name="port">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="handler">The default <see cref="T:Stumps.Http.IHttpHandler"/> executed when receiving traffic.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="handler"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        public HttpServer(ServerScheme scheme, int port, IHttpHandler handler)
        {

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            _listener = new HttpListener();
            _port = port;
            _scheme = scheme;
            _handler = handler;

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
        ~HttpServer()
        {
            Dispose();
        }

        /// <summary>
        ///     Occurs when the server processed an incomming HTTP request and returned the response to the client.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> RequestFinished;

        /// <summary>
        ///     Occurs after the server has finished processing the HTTP request, 
        ///     and has constructed a response, but before it returned to the client.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> RequestProcessed;

        /// <summary>
        ///     Occurs when the server receives an incomming HTTP request.
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
            get { return _port; }
        }

        /// <summary>
        ///     Gets the scheme used to listen for connections by the instance.
        /// </summary>
        /// <value>
        ///     The scheme used to listen for connections by the instance.
        /// </value>
        public ServerScheme Scheme
        {
            get { return _scheme; }
        }

        /// <summary>
        ///     Gets a value indicating whether the instance is started.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance is started; otherwise, <c>false</c>.
        /// </value>
        public bool Started
        {
            get { return _started; }
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

            var disposable = _listener as IDisposable;
            if (disposable != null)
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
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine("Type - " + ex.GetType() + " \nSource - " + ex.Source + " \nException - " + ex.ToString() + " \n\n");
            }
        }

        /// <summary>
        ///     Processes the incoming HTTP request asynchronously.
        /// </summary>
        /// <param name="asyncResult">The asynchronous result.</param>
        private async void ProcessAsyncRequest()
        {

            if (_listener == null)
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

                if (this.RequestReceived != null)
                {
                    this.RequestReceived(this, new StumpsContextEventArgs(stumpsContext));
                }

                // Process the request through the HTTP handler
                var processResult = await _handler.ProcessRequest(stumpsContext);

                if (this.RequestProcessed != null)
                {
                    this.RequestProcessed(this, new StumpsContextEventArgs(stumpsContext));
                }

                var abortConnection = processResult == ProcessHandlerResult.DropConnection;

                // End the request
                stumpsContext.EndResponse(abortConnection);

                if (this.RequestFinished != null)
                {
                    this.RequestFinished(this, new StumpsContextEventArgs(stumpsContext));
                }

            }
            catch (HttpListenerException e)
            {
                Console.WriteLine("Type - " + e.GetType() + " \nSource - " + e.Source + " \nException - " + e.ToString() + " \n\n");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Type - " + ex.GetType() + " \nSource - " + ex.Source + " \nException - " + ex.ToString() + " \n\n");
            }

        }

        /// <summary>
        ///     Wait for incoming HTTP connections.
        /// </summary>
        private void WaitForConnections()
        {

            if (_started && _listener.IsListening)
            {
                Task.Factory.StartNew(new Action(ProcessAsyncRequest));
            }

        }

    }

}