namespace Stumps
{

    using System;
    using System.Net;
    using System.Threading;
    using Stumps.Http;

    /// <summary>
    ///     A class that represents a proxy server.
    /// </summary>
    public sealed class StumpsServer : IDisposable
    {

        private readonly object _syncRoot;
        private readonly IStumpsManager _stumpsManager;
        private readonly Uri _externalHost;
        private readonly ServerDefaultResponse _defaultResponse;

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
        /// <param name="port">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="defaultResponse">The default response returned to a client when a matching <see cref="T:Stumps.Stump"/> is not found.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port" /> exceeds the allowed TCP port range.</exception>
        public StumpsServer(int port, ServerDefaultResponse defaultResponse) : this(port, null)
        {
            _defaultResponse = defaultResponse;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsServer" /> class.
        /// </summary>
        /// <param name="port">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="externalHostUri">The external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port" /> exceeds the allowed TCP port range.</exception>
        public StumpsServer(int port, Uri externalHostUri)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            _syncRoot = new object();
            _stumpsManager = new StumpsManager();

            _port = port;

            _externalHost = externalHostUri;
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
        /// Starts this instance of the proxy server.
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
                if (_externalHost != null)
                {
                    var proxyHandler = new ProxyHandler(_externalHost);
                    proxyHandler.ContextProcessed += (o, e) => Interlocked.Increment(ref _proxyCounter);
                    pipeline.Add(proxyHandler);
                }
                else
                {
                    // Setup the Service Unavailable HTTP handler
                    var stumpNotFoundHandler = new DefaultResponseHandler(_defaultResponse);
                    pipeline.Add(stumpNotFoundHandler);
                }

                _server = new HttpServer(_port, pipeline);
                _server.RequestFinishing += (o, e) => Interlocked.Increment(ref _requestCounter);
                _server.StartListening();

            }

        }

        /// <summary>
        ///     Stops this instance of the proxy server.
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

        ///// <summary>
        /////     Decodes the body of a based on the content encoding.
        ///// </summary>
        ///// <param name="part">The <see cref="T:Stumps.Proxy.RecordedContext"/> part containing the body to decode.</param>
        //private void DecodeBody(IRecordedContextPart part)
        //{

        //    var buffer = part.Body;
        //    var header = part.FindHeader("Content-Encoding");

        //    if (header != null)
        //    {
        //        var encoder = new ContentEncoder(header.Value);
        //        buffer = encoder.Decode(buffer);
        //    }

        //    part.Body = buffer;

        //}

        ///// <summary>
        /////     Records an incomming request.
        ///// </summary>
        ///// <param name="context">The <see cref="T:Stumps.Http.StumpsHttpContext"/> to record.</param>
        //private void RecordIncommingRequest(StumpsHttpContext context)
        //{

        //    var recordedContext = new RecordedContext();
        //    var recordedRequest = new RecordedRequest();

        //    recordedRequest.Body = StreamUtility.ConvertStreamToByteArray(context.Request.InputStream);
        //    recordedRequest.HttpMethod = context.Request.HttpMethod;
        //    recordedRequest.RawUrl = context.Request.RawUrl;

        //    foreach (var key in context.Request.Headers.AllKeys)
        //    {
        //        recordedRequest.Headers.Add(
        //            new HttpHeader
        //            {
        //                Name = key,
        //                Value = context.Request.Headers[key]
        //            });
        //    }

        //    DecodeBody(recordedRequest);

        //    recordedContext.Request = recordedRequest;

        //    _contextCache.Add(context.ContextId, recordedContext);

        //}

        ///// <summary>
        /////     Updates the response of an existing recorded request.
        ///// </summary>
        ///// <param name="context">The <see cref="T:Stumps.Http.StumpsHttpContext"/> to update.</param>
        //private void UpdateRecordedRequest(StumpsHttpContext context)
        //{

        //    RecordedContext recordedContext;

        //    if (!_contextCache.TryGetValue(context.ContextId, out recordedContext))
        //    {
        //        return;
        //    }

        //    _contextCache.Remove(context.ContextId);

        //    var recordedResponse = new RecordedResponse();

        //    recordedResponse.Body = StreamUtility.ConvertStreamToByteArray(context.Response.OutputStream);
        //    recordedResponse.StatusCode = context.Response.StatusCode;
        //    recordedResponse.StatusDescription = context.Response.StatusDescription;

        //    foreach (var key in context.Response.Headers.AllKeys)
        //    {
        //        recordedResponse.Headers.Add(
        //            new HttpHeader
        //            {
        //                Name = key,
        //                Value = context.Response.Headers[key]
        //            });
        //    }

        //    DecodeBody(recordedResponse);

        //    recordedContext.Response = recordedResponse;

        //    _environment.Recordings.Add(recordedContext);

        //}

    }

}