namespace Stumps {

    using System;
    using System.Collections.Generic;
    using Stumps.Logging;
    using Stumps.Proxy;
    using Stumps.Data;
    using Stumps.Web;

    public sealed class StumpsServer : IDisposable {

        private readonly object _syncRoot;
        private List<IStumpModule> _modules;
        private bool _disposed;

        private bool _started;

        public StumpsServer(Configuration configuration) {

            if ( configuration == null ) {
                throw new ArgumentNullException("configuration");
            }

            this.Configuration = configuration;
            _syncRoot = new object();

        }

        public Configuration Configuration { 
            get; 
            private set; 
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed when the modules are stopped.")]
        public void Start() {

            lock ( _syncRoot ) {

                if ( _started ) {
                    return;
                }

                _started = true;

                _modules = new List<IStumpModule>();

                var logger = new DebugLogger();

                var dataAccess = new DataAccess(this.Configuration.StoragePath);
                var host = new ProxyHost(logger, dataAccess);
                host.Load();

                var proxyServer = new ProxyServerModule(logger, host);

                var bootStrapper = new Bootstrapper(host);

                var webServer = new WebServerModule(logger, bootStrapper, this.Configuration.WebApiPort);

                _modules.Add(proxyServer);
                _modules.Add(webServer);

                StartModules();

            }

        }

        public void Stop() {

            lock ( _syncRoot ) {
                if ( !_started ) {
                    return;
                }

                _started = false;

                StopAndDisposeModules();

            }

        }

        private void StartModules() {
            foreach ( var module in _modules ) {
                module.Start();
            }
        }

        private void StopAndDisposeModules() {

            foreach ( var module in _modules ) {
                module.Shutdown();
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
