namespace Stumps
{

    using System;
    using System.Net;
    using System.Threading;
    using Stumps.Http;

    /// <summary>
    ///     A class that represents a Stumps server.
    /// </summary>
    public sealed class StumpsServer : IStumpsServer
    {

        private readonly object _syncRoot;
        private readonly IStumpsManager _stumpsManager;

        private HttpServer _server;
        private StumpsHandler _stumpsHandler;

        private FallbackResponse _defaultResponse;
        private Uri _remoteHttpServer;
        private int _port;
        private volatile bool _stumpsEnabled;

        private int _requestCounter;
        private int _proxyCounter;
        private int _stumpsCounter;

        private bool _disposed;
        private bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        public StumpsServer()
        {
            this.ListensOnPort = NetworkInformation.FindRandomOpenPort();
            _remoteHttpServer = null;

            _syncRoot = new object();
            _stumpsManager = new StumpsManager();

            this.DefaultResponse = FallbackResponse.Http503ServiceUnavailable;
            this.StumpsEnabled = true;

        }
        
        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        ~StumpsServer()
        {
            Dispose();
        }

        /// <summary>
        ///     Occurs when the server finishes processing an HTTP request.
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
        ///     Gets or sets the default response when a <see cref="T:Stumps.Stump"/> is not found, 
        ///     and a remote HTTP server is not available.
        /// </summary>
        /// <value>
        ///     The default response when a <see cref="T:Stumps.Stump"/> is not found, and a remote HTTP 
        ///     server is not available.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        public FallbackResponse DefaultResponse
        {
            get
            {
                return _defaultResponse;
            }

            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                _defaultResponse = value;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the server is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the server is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get { return _started; }
        }

        /// <summary>
        ///     Gets the port the HTTP server is using to listen for traffic.
        /// </summary>
        /// <value>
        ///     The port the HTTP server is using to listen for traffic.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is not a valid TCP port.</exception>
        public int ListensOnPort
        {
            get
            {
                return _port;
            }

            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                if (value < IPEndPoint.MinPort || value > IPEndPoint.MaxPort)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                _port = value;
            }
        }

        /// <summary>
        ///     Gets the remote HTTP that is contacted when a <see cref="T:Stumps.Stump" /> is unavailable to handle the incomming request.
        /// </summary>
        /// <value>
        ///     The remote HTTP that is contacted when a <see cref="T:Stumps.Stump" /> is unavailable to handle the incomming request.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The URI for the remote HTTP server is invalid.</exception>
        public Uri RemoteHttpServer
        {
            get
            {
                return _remoteHttpServer;
            }

            set
            {

                if (this.IsRunning)
                {
                    throw new InvalidOperationException(BaseResources.ServerIsRunning);
                }

                if (value == null)
                {
                    _remoteHttpServer = null;
                    return;
                }

                var isValid = value.AbsolutePath.Equals("/", StringComparison.Ordinal) &&
                              !string.IsNullOrEmpty(value.Authority) &&
                              (value.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                               value.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) &&
                              string.IsNullOrEmpty(value.UserInfo) && value.Port >= IPEndPoint.MinPort &&
                              value.Port <= IPEndPoint.MaxPort;

                if (!isValid)
                {
                    throw new ArgumentOutOfRangeException(BaseResources.InvalidUri);
                }

                _remoteHttpServer = value;

            }

        }

        /// <summary>
        ///     Gets the number of requests served with the proxy.
        /// </summary>
        /// <value>
        ///     The number of requests served with the proxy.
        /// </value>
        public int RequestsServedWithProxy
        {
            get { return _proxyCounter; }
        }

        /// <summary>
        ///     Gets the number requests served with a Stump.
        /// </summary>
        /// <value>
        ///     The number of requests served with a Stumps.
        /// </value>
        public int RequestsServedWithStump
        {
            get { return _stumpsCounter; }
        }

        /// <summary>
        /// Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        /// The count of Stumps in the collection.
        /// </value>
        public int StumpCount
        {
            get { return _stumpsManager.Count; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use stumps when serving requests.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use stumps when serving requests; otherwise, <c>false</c>.
        /// </value>
        public bool StumpsEnabled
        {
            get
            {
                return _stumpsEnabled;
            }

            set
            {
                _stumpsEnabled = value;
                UpdateStumpsEnabledFlag(value);
            }
        }

        /// <summary>
        ///     Gets the total number of requests served.
        /// </summary>
        /// <value>
        ///     The total number of requests served.
        /// </value>
        public int TotalRequestsServed
        {
            get { return _requestCounter; }
        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump" /> with a specified identifier to the collection.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump" />.</param>
        /// <returns>A new <see cref="T:Stumps.Stump"/> with the specified <paramref name="stumpId"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpId"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A <see cref="T:Stumps.Stump" /> with the same identifier already exists.</exception>
        public Stump AddNewStump(string stumpId)
        {
            var stump = new Stump(stumpId);
            _stumpsManager.AddStump(stump);
            return stump;
        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump" /> to the collection.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump" /> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A <see cref="T:Stumps.Stump" /> with the same identifier already exists.</exception>
        public void AddStump(Stump stump)
        {
            _stumpsManager.AddStump(stump);
        }

        /// <summary>
        ///     Deletes the specified stump from the collection.
        /// </summary>
        /// <param name="stumpId">The  unique identifier for the stump to remove.</param>
        public void DeleteStump(string stumpId)
        {
            _stumpsManager.DeleteStump(stumpId);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {

                _disposed = true;

                if (_started)
                {
                    this.Shutdown();
                }

                if (_stumpsManager != null)
                {
                    _stumpsManager.Dispose();
                }
            }

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> with the specified <paramref name="stumpId"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stump is not found.
        /// </remarks>
        public Stump FindStump(string stumpId)
        {
            return _stumpsManager.FindStump(stumpId);
        }
        
        /// <summary>
        ///     Stops this instance of the Stumps server.
        /// </summary>
        public void Shutdown()
        {

            lock (_syncRoot)
            {

                if (!_started)
                {
                    return;
                }

                _started = false;
                _server.StopListening();

                _server.Dispose();
                _server = null;

            }

        }

        /// <summary>
        ///     Starts this instance of the Stumps server.
        /// </summary>
        public void Start()
        {

            lock (_syncRoot)
            {

                if (_started)
                {
                    return;
                }

                _started = true;

                // Setup the pipeline HTTP handler
                var pipeline = new HttpPipelineHandler();

                // Setup the Stump HTTP handler
                _stumpsHandler = new StumpsHandler(_stumpsManager);
                _stumpsHandler.Enabled = this.StumpsEnabled;

                pipeline.Add(_stumpsHandler);

                // Setup the Proxy HTTP handler
                if (_remoteHttpServer != null)
                {
                    var proxyHandler = new ProxyHandler(_remoteHttpServer);
                    pipeline.Add(proxyHandler);
                }
                else
                {
                    // Setup the Service Unavailable HTTP handler
                    var stumpNotFoundHandler = new FallbackResponseHandler(_defaultResponse);
                    pipeline.Add(stumpNotFoundHandler);
                }

                _server = new HttpServer(_port, pipeline);

                _server.RequestFinished += ServerRequestFinished;
                _server.RequestProcessed += ServerRequestProcessed;
                _server.RequestReceived += ServerRequestStarted;

                _server.StartListening();

            }

        }

        /// <summary>
        /// Handles the requestFinishing event of the server instance.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Stumps.StumpsContextEventArgs"/> instance containing the event data.</param>
        private void ServerRequestFinished(object sender, StumpsContextEventArgs e)
        {

            // Increment the request counter
            Interlocked.Increment(ref _requestCounter);

            if (e.ResponseOrigin == HttpResponseOrigin.RemoteServer)
            {
                // Increment the proxy counter
                Interlocked.Increment(ref _proxyCounter);
            }
            else if (e.ResponseOrigin == HttpResponseOrigin.Stump)
            {
                // Increment the Stumps counter
                Interlocked.Increment(ref _stumpsCounter);
            }

            // Raise the processed event
            if (this.RequestFinished != null)
            {
                this.RequestFinished(this, e);
            }

        }

        /// <summary>
        /// Handles the RequestProcessed event of the server instance.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StumpsContextEventArgs"/> instance containing the event data.</param>
        private void ServerRequestProcessed(object sender, StumpsContextEventArgs e)
        {

            // Raise the request processed event
            if (this.RequestProcessed != null)
            {
                this.RequestProcessed(this, e);
            }

        }

        /// <summary>
        ///     Handles the RequestStarted event of the server instance.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="T:Stumps.StumpsContextEventArgs"/> instance containing the event data.</param>
        private void ServerRequestStarted(object sender, StumpsContextEventArgs e)
        {

            // Raise the request received event
            if (this.RequestReceived != null)
            {
                this.RequestReceived(this, e);
            }

        }

        /// <summary>
        ///     Updates the enabled flag of the Stumps handler.
        /// </summary>
        /// <param name="enabled">If set to <c>true</c>, Stumps are enabled.</param>
        private void UpdateStumpsEnabledFlag(bool enabled)
        {

            if (_server != null && _stumpsHandler != null)
            {
                _stumpsHandler.Enabled = enabled;
            }

        }

    }

}