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
        private readonly Uri _proxyHost;
        private readonly FallbackResponse _defaultResponse;

        private readonly int _port;

        private bool _disposed;
        private HttpServer _server;
        private bool _started;

        private int _requestCounter;
        private int _proxyCounter;
        private int _stumpsCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.StumpsServer" /> class.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="defaultResponse">The default response returned to a client when a matching <see cref="T:Stumps.Stump"/> is not found.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        public StumpsServer(int listeningPort, FallbackResponse defaultResponse) : this(listeningPort, null)
        {
            _defaultResponse = defaultResponse;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsServer" /> class.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="proxyHostUri">The external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        public StumpsServer(int listeningPort, Uri proxyHostUri)
        {
            if (listeningPort < IPEndPoint.MinPort || listeningPort > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("listeningPort");
            }

            _syncRoot = new object();
            _stumpsManager = new StumpsManager();

            _port = listeningPort;

            _proxyHost = proxyHostUri;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        ~StumpsServer()
        {
            Dispose();
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
        public int ListeningPort
        {
            get { return _port; }
        }

        /// <summary>
        ///     Gets the external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.
        /// </summary>
        /// <value>
        ///     The external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.
        /// </value>
        public Uri ProxyHostUri
        {
            get { return _proxyHost; }
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
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {

                _disposed = true;

                if (_started)
                {
                    this.Stop();
                }

                if (_stumpsManager != null)
                {
                    _stumpsManager.Dispose();
                }
            }

            GC.SuppressFinalize(this);

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
                var stumpsHandler = new StumpsHandler(_stumpsManager);
                stumpsHandler.ContextProcessed += (o, e) => Interlocked.Increment(ref _stumpsCounter);
                pipeline.Add(stumpsHandler);

                // Setup the Proxy HTTP handler
                if (_proxyHost != null)
                {
                    var proxyHandler = new ProxyHandler(_proxyHost);
                    proxyHandler.ContextProcessed += (o, e) => Interlocked.Increment(ref _proxyCounter);
                    pipeline.Add(proxyHandler);
                }
                else
                {
                    // Setup the Service Unavailable HTTP handler
                    var stumpNotFoundHandler = new FallbackResponseHandler(_defaultResponse);
                    pipeline.Add(stumpNotFoundHandler);
                }

                _server = new HttpServer(_port, pipeline);
                _server.RequestFinishing += (o, e) => Interlocked.Increment(ref _requestCounter);
                _server.StartListening();

            }

        }

        /// <summary>
        ///     Stops this instance of the Stumps server.
        /// </summary>
        public void Stop()
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

    }

}