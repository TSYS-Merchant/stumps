namespace Stumps.Web
{

    using System;
    using System.Net;
    using Nancy.Bootstrapper;
    using Nancy.Hosting.Self;
    using Stumps.Logging;

    /// <summary>
    ///     A class representing the web server Stumps module.
    /// </summary>
    internal sealed class WebServerModule : IStumpModule
    {

        private readonly ILogger _logger;
        private readonly NancyHost _server;
        private bool _disposed;
        private bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebServerModule"/> class.
        /// </summary>
        /// <param name="logger">The logger used by the instance.</param>
        /// <param name="bootstrapper">The Nancy bootstrapper.</param>
        /// <param name="port">The port used to listen for traffic.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="logger"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> is invalid.</exception>
        public WebServerModule(ILogger logger, INancyBootstrapper bootstrapper, int port)
        {

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            _logger = logger;

            var urlString = string.Format(
                System.Globalization.CultureInfo.InvariantCulture, "http://localhost:{0}/", port);

            this._server = bootstrapper == null ? new NancyHost(new Uri(urlString)) : new NancyHost(bootstrapper, new Uri(urlString));

        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {
                _disposed = true;
                this.Shutdown();
                _server.Dispose();
            }

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Starts the instance of the module.
        /// </summary>
        public void Start()
        {
            if (_started)
            {
                return;
            }

            _server.Start();
            _logger.LogInfo("Web server started.");

            _started = true;
        }

        /// <summary>
        ///     Shuts down the instance of the module.
        /// </summary>
        public void Shutdown()
        {
            if (!_started)
            {
                return;
            }

            _server.Stop();
            _logger.LogInfo("Web server shut down.");

            _started = false;
        }

    }

}