namespace Stumps.Web
{
    using System;
    using System.Net;
    using Nancy.Hosting.Self;
    using Stumps.Server;

    /// <summary>
    ///     A class representing Stumps web server.
    /// </summary>
    public sealed class StumpsWebServer : IDisposable
    {
        private readonly NancyHost _server;
        private bool _disposed;
        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Stumps.Web.StumpsWebServer" /> class.
        /// </summary>
        /// <param name="host">The Stumps Server host.</param>
        /// <param name="port">The port used to listen for traffic.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="host"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port" /> is invalid.</exception>
        /// <exception cref="System.InvalidOperationException">The port is already being used.</exception>
        public StumpsWebServer(IStumpsHost host, int port)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (NetworkInformation.IsPortBeingUsed(port))
            {
                throw new InvalidOperationException("The port is already being used.");
            }

            var urlString = string.Format(
                System.Globalization.CultureInfo.InvariantCulture, "http://localhost:{0}/", port);

            var bootStrapper = new Bootstrapper(host);
            _server = new NancyHost(bootStrapper, new Uri(urlString));
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
        ///     Shuts down the instance of the module.
        /// </summary>
        public void Shutdown()
        {
            if (!_started)
            {
                return;
            }

            _server.Stop();

            _started = false;
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

            _started = true;
        }
    }
}