namespace Stumps.Web {

    using System;
    using Nancy.Bootstrapper;
    using Nancy.Hosting.Self;
    using Stumps.Logging;
    using Stumps.Data;

    internal sealed class WebServerModule : IStumpModule {

        private NancyHost _server;
        private ILogger _logger;
        private bool _disposed;
        private bool _started;

        public WebServerModule(ILogger logger, INancyBootstrapper bootstrapper) {

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            _logger = logger;

            if ( bootstrapper == null ) {
                _server = new NancyHost(new Uri("http://localhost:8888/"));
            }
            else {
                _server = new NancyHost(bootstrapper, new Uri("http://localhost:8888/"));
            }

        }

        public void Start() {
            if ( _started ) {
                return;
            }

            _server.Start();

            _started = true;
        }

        public void Stop() {
            if ( !_started ) {
                return;
            }

            _server.Stop();

            _started = false;
        }

        #region IDisposable Members

        public void Dispose() {

            if ( !_disposed ) {
                _disposed = true;
                this.Stop();
                _server.Dispose();
            }

            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
