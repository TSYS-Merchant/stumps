using Stumps.Utility;

namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using Stumps.Logging;

    internal sealed class ProxyServer : IDisposable {

        private HttpServer _server;
        private readonly ILogger _logger;
        private readonly ProxyEnvironment _environment;
        private readonly object _syncRoot = new object();
        private readonly Dictionary<Guid, RecordedContext> _contextCache;

        private bool _started;
        private bool _disposed;

        public ProxyServer(ProxyEnvironment environment, ILogger logger) {

            if ( environment == null ) {
                throw new ArgumentNullException("environment");
            }

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            _contextCache = new Dictionary<Guid, RecordedContext>();

            _environment = environment;
            _logger = logger;

        }

        public ProxyEnvironment Environment {
            get { return _environment; }
        }

        public bool IsRunning {
            get { return _started; }
        }

        public void Start() {

            lock ( _syncRoot ) {

                if ( _started ) {
                    return;
                }

                _started = true;
                _environment.IsRunning = true;

                var pipeline = new HttpPipelineHandler();

                pipeline.Add(new StumpsHandler(_environment, _logger));
                pipeline.Add(new ProxyHandler(_environment, _logger));

                _server = new HttpServer(_environment.Port, pipeline, _logger);

                _server.RequestStarting += (o, e) => {
                    _environment.IncrementRequestsServed();

                    if ( _environment.RecordTraffic ) {
                        CreateRecordedContext(e.Context);
                    }
                };

                _server.RequestFinishing += (o, e) => {
                    if ( _environment.RecordTraffic ) {
                        CompleteRecordedContext(e.Context);
                    }
                };

                _server.StartListening();

            }

        }

        public void Stop() {

            lock ( _syncRoot ) {

                if ( !_started ) {
                    return;
                }

                _started = false;
                _environment.IsRunning = false;

                if ( _environment.RecordTraffic ) {
                    _environment.RecordTraffic = false;
                }

                _server.StopListening();

                _contextCache.Clear();
                _environment.Recordings.Clear();

                _server.Dispose();
                _server = null;

            }

        }

        private void CreateRecordedContext(StumpsHttpContext context) {

            var recordedContext = new RecordedContext();
            var recordedRequest = new RecordedRequest();

            recordedRequest.Body = StreamUtility.ConvertStreamToByteArray(context.Request.InputStream);
            recordedRequest.HttpMethod = context.Request.HttpMethod;
            recordedRequest.RawUrl = context.Request.RawUrl;

            foreach ( var key in context.Request.Headers.AllKeys ) {
                recordedRequest.Headers.Add(new HttpHeader() {
                    Name = key,
                    Value = context.Request.Headers[key]
                });
            }

            DecodeBody(recordedRequest);

            recordedContext.Request = recordedRequest;

            _contextCache.Add(context.ContextId, recordedContext);

        }

        private void CompleteRecordedContext(StumpsHttpContext context) {

            RecordedContext recordedContext = null;

            if ( !_contextCache.TryGetValue(context.ContextId, out recordedContext) ) {
                return;
            }

            _contextCache.Remove(context.ContextId);

            var recordedResponse = new RecordedResponse();

            recordedResponse.Body = StreamUtility.ConvertStreamToByteArray(context.Response.OutputStream);
            recordedResponse.StatusCode = context.Response.StatusCode;
            recordedResponse.StatusDescription = context.Response.StatusDescription;

            foreach ( var key in context.Response.Headers.AllKeys ) {
                recordedResponse.Headers.Add(new HttpHeader() {
                    Name = key,
                    Value = context.Response.Headers[key]
                });
            }

            DecodeBody(recordedResponse);

            recordedContext.Response = recordedResponse;

            _environment.Recordings.Add(recordedContext);

        }

        private static void DecodeBody(IRecordedContextPart part) {

            var buffer = part.Body;
            var header = part.FindHeader("Content-Encoding");

            if ( header != null ) {
                var encoder = new ContentEncoding(header.Value);
                buffer = encoder.Decode(buffer);
            }

            part.Body = buffer;

        }

        #region IDisposable Members

        public void Dispose() {

            if ( !_disposed ) {

                _disposed = true;

                if ( _started ) {
                    this.Stop();
                }

            }

            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
