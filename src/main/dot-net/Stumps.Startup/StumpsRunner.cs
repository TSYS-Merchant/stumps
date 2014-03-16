namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using Stumps.Data;
    using Stumps.Logging;
    using Stumps.Proxy;
    using Stumps.Server;
    using Stumps.Server.Data;
    using Stumps.Server.Logging;
    using Stumps.Server.Proxy;
    using Stumps.Web;

    /// <summary>
    ///     A class that represents the core Stumps server.
    /// </summary>
    public sealed class StumpsRunner : IDisposable
    {

        private readonly object _syncRoot;
        private bool _disposed;
        private List<IStumpModule> _modules;

        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="T:Stumps.StumpsConfiguration"/> used to initialize the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
        public StumpsRunner(StumpsConfiguration configuration)
        {

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            this.Configuration = configuration;
            _syncRoot = new object();

        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        ~StumpsRunner()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the <see cref="T:Stumps.StumpsConfiguration"/> used to initialize the instance.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.StumpsConfiguration"/> used to initialize the instance.
        /// </value>
        public StumpsConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Starts all proxy servers that are set to automatically start and the REST API.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Objects are disposed when the modules are stopped.")]
        public void Start()
        {

            // Prevent multiple simultaneous requests to start or stop the instance. 
            lock (_syncRoot)
            {

                if (_started)
                {
                    return;
                }

                _started = true;

                var logger = new DebugLogger();

                // Initialize a new instance of the data access layer.
                var dataAccess = new DataAccess(this.Configuration.StoragePath);

                // Initialize and load a new instance of the proxy host.
                var host = new ProxyHost(logger, dataAccess);
                host.Load();

                // Initialize a new proxy server module.
                var proxyServer = new ProxyServerModule(logger, host);

                // Initialize the Nancy boot strapper.
                var bootStrapper = new Bootstrapper(host);

                // Initialize the Nancy web server module.
                var webServer = new WebServerModule(logger, bootStrapper, this.Configuration.WebApiPort);

                _modules = new List<IStumpModule>
                {
                    proxyServer,
                    webServer
                };

                StartModules();

            }

        }

        /// <summary>
        ///     Stops all running proxy servers and the REST API.
        /// </summary>
        public void Stop()
        {

            lock (_syncRoot)
            {
                if (!_started)
                {
                    return;
                }

                _started = false;

                StopAndDisposeModules();

            }

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
                    this.Stop();
                }

            }

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Starts all supported modules.
        /// </summary>
        private void StartModules()
        {
            foreach (var module in _modules)
            {
                module.Start();
            }
        }

        /// <summary>
        ///     Stops and disposes all modules.
        /// </summary>
        private void StopAndDisposeModules()
        {

            foreach (var module in _modules)
            {
                module.Shutdown();
                module.Dispose();
            }

            _modules.Clear();

        }

    }

}