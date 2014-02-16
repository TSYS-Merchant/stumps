namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using Stumps.Logging;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents a proxy server.
    /// </summary>
    internal sealed class ProxyServer : IDisposable
    {

        private readonly Dictionary<Guid, RecordedContext> _contextCache;
        private readonly ProxyEnvironment _environment;
        private readonly ILogger _logger;
        private readonly object _syncRoot = new object();
        private bool _disposed;
        private HttpServer _server;
        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.ProxyServer"/> class.
        /// </summary>
        /// <param name="environment">The environment for the proxy server.</param>
        /// <param name="logger">The logger used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="environment"/> is <c>null</c>.
        /// or
        /// <paramref name="logger"/> is <c>null</c>.
        /// </exception>
        public ProxyServer(ProxyEnvironment environment, ILogger logger)
        {

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _contextCache = new Dictionary<Guid, RecordedContext>();

            _environment = environment;
            _logger = logger;

        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Proxy.ProxyServer"/> class.
        /// </summary>
        ~ProxyServer()
        {
            Dispose();
        }

        /// <summary>
        ///     Gets the environment for the proxy server.
        /// </summary>
        /// <value>
        ///     The environment for the proxy server.
        /// </value>
        public ProxyEnvironment Environment
        {
            get { return _environment; }
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

            }

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Starts this instance of the proxy server.
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
                _environment.IsRunning = true;

                var pipeline = new HttpPipelineHandler();

                pipeline.Add(new StumpsHandler(_environment, _logger));
                pipeline.Add(new ProxyHandler(_environment, _logger));

                _server = new HttpServer(_environment.Port, pipeline, _logger);

                _server.RequestStarting += (o, e) =>
                {
                    _environment.IncrementRequestsServed();

                    if (_environment.RecordTraffic)
                    {
                        RecordIncommingRequest(e.Context);
                    }
                };

                _server.RequestFinishing += (o, e) =>
                {
                    if (_environment.RecordTraffic)
                    {
                        UpdateRecordedRequest(e.Context);
                    }
                };

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
                _environment.IsRunning = false;

                if (_environment.RecordTraffic)
                {
                    _environment.RecordTraffic = false;
                }

                _server.StopListening();

                _contextCache.Clear();
                _environment.Recordings.Clear();

                _server.Dispose();
                _server = null;

            }

        }

        /// <summary>
        ///     Decodes the body of a based on the content encoding.
        /// </summary>
        /// <param name="part">The <see cref="T:Stumps.Proxy.RecordedContext"/> part containing the body to decode.</param>
        private void DecodeBody(IRecordedContextPart part)
        {

            var buffer = part.Body;
            var header = part.FindHeader("Content-Encoding");

            if (header != null)
            {
                var encoder = new ContentEncoding(header.Value);
                buffer = encoder.Decode(buffer);
            }

            part.Body = buffer;

        }

        /// <summary>
        ///     Records an incomming request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Http.StumpsHttpContext"/> to record.</param>
        private void RecordIncommingRequest(StumpsHttpContext context)
        {

            var recordedContext = new RecordedContext();
            var recordedRequest = new RecordedRequest();

            recordedRequest.Body = StreamUtility.ConvertStreamToByteArray(context.Request.InputStream);
            recordedRequest.HttpMethod = context.Request.HttpMethod;
            recordedRequest.RawUrl = context.Request.RawUrl;

            foreach (var key in context.Request.Headers.AllKeys)
            {
                recordedRequest.Headers.Add(
                    new HttpHeader
                    {
                        Name = key,
                        Value = context.Request.Headers[key]
                    });
            }

            DecodeBody(recordedRequest);

            recordedContext.Request = recordedRequest;

            _contextCache.Add(context.ContextId, recordedContext);

        }


        /// <summary>
        ///     Updates the response of an existing recorded request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Http.StumpsHttpContext"/> to update.</param>
        private void UpdateRecordedRequest(StumpsHttpContext context)
        {

            RecordedContext recordedContext;

            if (!_contextCache.TryGetValue(context.ContextId, out recordedContext))
            {
                return;
            }

            _contextCache.Remove(context.ContextId);

            var recordedResponse = new RecordedResponse();

            recordedResponse.Body = StreamUtility.ConvertStreamToByteArray(context.Response.OutputStream);
            recordedResponse.StatusCode = context.Response.StatusCode;
            recordedResponse.StatusDescription = context.Response.StatusDescription;

            foreach (var key in context.Response.Headers.AllKeys)
            {
                recordedResponse.Headers.Add(
                    new HttpHeader
                    {
                        Name = key,
                        Value = context.Response.Headers[key]
                    });
            }

            DecodeBody(recordedResponse);

            recordedContext.Response = recordedResponse;

            _environment.Recordings.Add(recordedContext);

        }

    }

}