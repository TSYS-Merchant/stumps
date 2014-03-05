namespace Stumps.Proxy
{

    using System;
    using Stumps.Logging;

    /// <summary>
    ///     A class that represents a Stump server module that manages multiple proxy servers.
    /// </summary>
    internal sealed class ProxyServerModule : IStumpModule
    {

        private bool _disposed;
        private IProxyHost _host;
        private ILogger _logger;
        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.ProxyServerModule"/> class.
        /// </summary>
        /// <param name="logger">The logger used by the instance.</param>
        /// <param name="proxyHost">The proxy host.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="logger"/> is <c>null</c>.
        /// or
        /// <paramref name="proxyHost"/> is <c>null</c>.
        /// </exception>
        public ProxyServerModule(ILogger logger, IProxyHost proxyHost)
        {

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            _logger = logger;
            _host = proxyHost;

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Stumps.Proxy.ProxyServerModule"/> class.
        /// </summary>
        ~ProxyServerModule()
        {
            Dispose();
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
                    this.Shutdown();
                }

                _host.Dispose();
                _host = null;

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

            _host.Start();

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

            _host.Shutdown();

            _started = false;

        }

    }

}