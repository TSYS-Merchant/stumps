namespace Stumps {

    using System;
    using System.Collections.Generic;
    using Stumps.Logging;
    using Stumps.Proxy;
    using Stumps.Data;
    using Stumps.Web;

    public sealed class StumpsServer : IDisposable {

        private readonly object _syncRoot = new object();
        private List<IStumpModule> _modules;
        private bool _disposed;

        private bool _started;

        public void Start() {

            lock ( _syncRoot ) {

                if ( _started ) {
                    return;
                }

                _started = true;

                _modules = new List<IStumpModule>();

                var logger = new DebugLogger();
                var dataAccess = new DataAccess();
                var host = new ProxyHost(logger, dataAccess);
                host.Load();

                var proxyServer = new ProxyServerModule(logger, host);

                var bootStrapper = new Bootstrapper(host);

                var webServer = new WebServerModule(logger, bootStrapper);

                _modules.Add(proxyServer);
                _modules.Add(webServer);

                startModules();

            }

        }

        public void Stop() {

            lock ( _syncRoot ) {
                if ( !_started ) {
                    return;
                }

                _started = false;

                stopAndDisposeModules();

            }

        }

        private void startModules() {
            foreach ( var module in _modules ) {
                module.Start();
            }
        }

        private void stopAndDisposeModules() {

            foreach ( var module in _modules ) {
                module.Stop();
                module.Dispose();
            }

            _modules.Clear();

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
