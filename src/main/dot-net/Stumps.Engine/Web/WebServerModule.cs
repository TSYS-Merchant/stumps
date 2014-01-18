namespace Stumps.Web {

    using System;
    using System.Net;
    using Nancy.Bootstrapper;
    using Nancy.Hosting.Self;
    using Stumps.Logging;
    using Stumps.Data;

    internal sealed class WebServerModule : IStumpModule {

        private readonly NancyHost _server;
        private readonly ILogger _logger;
        private bool _disposed;
        private bool _started;

        public WebServerModule(ILogger logger, INancyBootstrapper bootstrapper, int port) {

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            if ( port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort ) {
                throw new ArgumentOutOfRangeException("port");
            }

            _logger = logger;

            var urlString = String.Format(System.Globalization.CultureInfo.InvariantCulture, "http://localhost:{0}/", port);

            if ( bootstrapper == null ) {
                _server = new NancyHost(new Uri(urlString));
            }
            else {
                _server = new NancyHost(bootstrapper, new Uri(urlString));
            }

        }

        public void Start() {
            if ( _started ) {
                return;
            }

            _server.Start();
            _logger.LogInfo("Web server started.");

            _started = true;
        }

        public void Shutdown() {
            if ( !_started ) {
                return;
            }

            _server.Stop();
            _logger.LogInfo("Web server shut down.");

            _started = false;
        }

        #region IDisposable Members

        public void Dispose() {

            if ( !_disposed ) {
                _disposed = true;
                this.Shutdown();
                _server.Dispose();
            }

            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
