//namespace Stumps.Http {

//    using System;
//    using System.Net;
//    using System.Threading;
//    using System.Threading.Tasks;

//    using Stumps.Logging;

//    internal sealed class HttpServer : IDisposable {

//        private ILogger _logger;
//        private IHttpHandler _handler;
//        private int _port;
//        private bool _started;
//        private System.Net.HttpListener _listener;
//        private int _threadCount;

//        public HttpServer(int port, IHttpHandler handler, ILogger logger) {

//            if ( logger == null ) {
//                throw new ArgumentNullException("logger");
//            }

//            if ( port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort ) {
//                throw new ArgumentOutOfRangeException("port");
//            }

//            _listener = new HttpListener();
//            _logger = logger;
//            _port = port;
//            _handler = handler;

//        }

//        public bool Started {
//            get { return _started; }
//        }

//        public async void StartListening() {

//            if ( _started ) {
//                return;
//            }

//            _started = true;

//            _listener = new HttpListener();

//            var url = String.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.HttpServerPattern, _port);

//            _listener.Prefixes.Add(url);
//            _listener.Start();
//            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

//            while ( _started ) {
//                try {
//                    var context = await _listener.GetContextAsync();
//                    var stumpsContext = new StumpsHttpContext(context);

//                    Task.Run(() => {
//                        var count = Interlocked.Increment(ref _threadCount);
//                        _logger.LogInfo("=> " + count.ToString() + " => " + context.Request.RawUrl);
//                        processRequestAsync(stumpsContext);
//                        _logger.LogInfo("<= " + count.ToString() + " <= " + context.Request.RawUrl);
//                        count = Interlocked.Decrement(ref _threadCount);
//                    });
//                }
//                catch ( HttpListenerException ) {
//                }
//                catch ( InvalidOperationException ) {
//                }
//            }

//        }

//        public void StopListening() {
//            try {
//                _started = false;
//                _listener.Stop();
//            }
//            catch ( ObjectDisposedException ) {
//            }
//        }

//        private void processRequestAsync(StumpsHttpContext context) {
//            _handler.ProcessRequest(context);
//            context.End();
//            context.Dispose();
//        }

//        #region IDisposable Members

//        public void Dispose() {

//            if ( _listener.IsListening ) {
//                _listener.Stop();
//            }

//            var disposable = _listener as IDisposable;
//            if ( disposable != null ) {
//                disposable.Dispose();
//            }

//        }

//        #endregion

//    }

//}
