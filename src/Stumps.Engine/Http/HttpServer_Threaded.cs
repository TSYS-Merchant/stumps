//namespace Stumps.Http {

//    using System;
//    using System.Net;
//    using System.Threading;
//    using Stumps.Logging;

//    internal sealed class HttpServer : IDisposable {

//        private ILogger _logger;
//        private IHttpHandler _handler;
//        private HttpListener _listener;
//        private int _port;
//        private bool _started;
//        private Thread _thread;
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

//        public void StartListening() {

//            if ( _started ) {
//                return;
//            }

//            _started = true;

//            _listener = new HttpListener();

//            var url = String.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.HttpServerPattern, _port);

//            _listener.Prefixes.Add(url);
//            _listener.Start();
//            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

//            _thread = new Thread(waitForConnections);
//            _thread.Start();


//        }

//        public void StopListening() {
//            try {
//                _started = false;
//                _listener.Stop();
//                _thread.Join();
//            }
//            catch ( ObjectDisposedException ) {
//            }
//        }

//        private void waitForConnections() {

//            while ( _started && _listener.IsListening ) {
//                var result = _listener.BeginGetContext(processAsyncRequest, null);
//                result.AsyncWaitHandle.WaitOne();
//            }

//        }

//        private void processAsyncRequest(IAsyncResult asyncResult) {

//            if ( _listener == null ) {
//                return;
//            }

//            try {
//                ThreadPool.QueueUserWorkItem((o) => {

//                    var context = (HttpListenerContext) o;

//                    var currentThreadCount = Interlocked.Increment(ref _threadCount);
//                    _logger.LogInfo("=> " + currentThreadCount.ToString() + ":" + Thread.CurrentThread.ManagedThreadId.ToString() + " => " + context.Request.RawUrl);

//                    var stumpsContext = new StumpsHttpContext(context);

//                    _handler.ProcessRequest(stumpsContext);
//                    stumpsContext.End();
//                    stumpsContext.Dispose();

//                    Interlocked.Decrement(ref _threadCount);
//                    _logger.LogInfo("<= " + currentThreadCount.ToString() + ":" + Thread.CurrentThread.ManagedThreadId.ToString() + " <= " + context.Request.RawUrl);


//                }, _listener.EndGetContext(asyncResult));

//            }
//            catch ( HttpListenerException ) {
//            }
//            catch ( InvalidOperationException ) {
//            }

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
