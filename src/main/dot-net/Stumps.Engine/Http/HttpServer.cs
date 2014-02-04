namespace Stumps.Http {

    using System;
    using System.Globalization;
    using System.Net;
    using Stumps.Logging;
    using System.Threading;

    internal sealed class HttpServer : IDisposable {

        private readonly ILogger _logger;
        private readonly IHttpHandler _handler;
        private readonly int _port;
        private HttpListener _listener;
        private bool _started;
        private Thread _thread;

        public event EventHandler<StumpsContextEventArgs> RequestStarting;
        public event EventHandler<StumpsContextEventArgs> RequestFinishing;

        public HttpServer(int port, IHttpHandler handler, ILogger logger) {

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            if ( handler == null ) {
                throw new ArgumentNullException("handler");
            }

            if ( port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort ) {
                throw new ArgumentOutOfRangeException("port");
            }

            _listener = new HttpListener();
            _logger = logger;
            _port = port;
            _handler = handler;

        }

        public int Port {
            get { return _port; }
        }

        public bool Started {
            get { return _started; }
        }

        public void StartListening() {

            if ( _started ) {
                return;
            }

            _started = true;

            _listener = new HttpListener();

            var url = String.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.HttpServerPattern, _port);

            _listener.Prefixes.Add(url);
            _listener.Start();
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            _thread = new Thread(WaitForConnections);
            _thread.Start();

        }

        public void StopListening() {
            try {
                _started = false;
                _listener.Stop();
                _thread.Join();
            }
            catch ( ObjectDisposedException ) {
            }
        }

        private void ProcessAsyncRequest(IAsyncResult asyncResult) {

            if ( _listener == null ) {
                return;
            }

            try {
                var context = _listener.EndGetContext(asyncResult);

                _logger.LogInfo("=> " + Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) + " => " + context.Request.RawUrl);

                StumpsHttpContext stumpsContext = null;

                try {
                    stumpsContext = new StumpsHttpContext(context);

                    if ( this.RequestStarting != null ) {
                        this.RequestStarting(this, new StumpsContextEventArgs(stumpsContext));
                    }

                    _handler.ProcessRequest(stumpsContext);

                    if ( this.RequestFinishing != null ) {
                        this.RequestFinishing(this, new StumpsContextEventArgs(stumpsContext));
                    }

                    // Set the status code
                    ((StumpsHttpResponse) stumpsContext.Response).ListenerResponse.StatusCode =
                        stumpsContext.Response.StatusCode;

                    ((StumpsHttpResponse) stumpsContext.Response).ListenerResponse.StatusDescription =
                        stumpsContext.Response.StatusDescription;

                    // Use HTTP chunked transfer encoding if requested
                    ((StumpsHttpResponse) stumpsContext.Response).ListenerResponse.SendChunked =
                        stumpsContext.Response.SendChunked;

                    ((StumpsHttpResponse) stumpsContext.Response).ListenerResponse.ContentLength64 =
                        stumpsContext.Response.OutputStream.Length;

                    stumpsContext.End();
                }
                finally {
                    if ( stumpsContext != null ) {
                        stumpsContext.Dispose();
                    }
                }

                _logger.LogInfo("<= " + Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) + " <= " + context.Request.RawUrl);

            }
            catch ( HttpListenerException ) {
            }
            catch ( InvalidOperationException ) {
            }

        }

        private void WaitForConnections() {

            while ( _started && _listener.IsListening ) {
                var result = _listener.BeginGetContext(ProcessAsyncRequest, null);
                result.AsyncWaitHandle.WaitOne();
            }

        }

        #region IDisposable Members

        public void Dispose() {

            if ( _listener.IsListening ) {
                _listener.Stop();
            }

            var disposable = _listener as IDisposable;
            if ( disposable != null ) {
                disposable.Dispose();
            }

        }

        #endregion

    }

}
