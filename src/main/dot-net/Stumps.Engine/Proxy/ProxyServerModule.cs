namespace Stumps.Proxy
{

    using System;
    using Stumps.Logging;

    internal sealed class ProxyServerModule : IStumpModule
    {

        private bool _disposed;
        private IProxyHost _host;
        private ILogger _logger;
        private bool _started;

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

        public void Start()
        {

            if (_started)
            {
                return;
            }

            _host.Start();

            _started = true;

        }

        public void Shutdown()
        {

            if (!_started)
            {
                return;
            }

            _host.Shutdown();

            _started = false;

        }

        #region IDisposable Members

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

        #endregion
    }

}