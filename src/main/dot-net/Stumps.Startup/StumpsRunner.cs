namespace Stumps
{
    using System;
    using Stumps.Server;
    using Stumps.Server.Data;
    using Stumps.Web;

    /// <summary>
    ///     A class that represents the core Stumps server.
    /// </summary>
    public sealed class StumpsRunner : IDisposable
    {
        private readonly object _syncRoot;
        private bool _disposed;

        private StumpsHost _host;
        private StumpsWebServer _webServer;

        private bool _started;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        /// <param name="configuration">The <see cref="T:Stumps.Server.StumpsConfiguration"/> used to initialize the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
        public StumpsRunner(StumpsConfiguration configuration)
        {
            this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _syncRoot = new object();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.StumpsServer"/> class.
        /// </summary>
        ~StumpsRunner() => Dispose();

        /// <summary>
        /// Gets the <see cref="T:Stumps.Server.StumpsConfiguration"/> used to initialize the instance.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.Server.StumpsConfiguration"/> used to initialize the instance.
        /// </value>
        public StumpsConfiguration Configuration
        {
            get;
        }

        /// <summary>
        ///     Stops all running proxy servers and the REST API.
        /// </summary>
        public void Shutdown()
        {
            lock (_syncRoot)
            {
                if (!_started)
                {
                    return;
                }

                _started = false;

                _webServer.Shutdown();
                _host.Shutdown();

                _host.Dispose();
                _webServer.Dispose();
            }
        }

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

                // Initialize a new instance of the data access layer.
                var dataAccess = new DataAccess(this.Configuration.StoragePath);

                var factory = new ServerFactory();

                // Initialize and load a new instance of the proxy host.
                _host = new StumpsHost(factory, dataAccess);
                _host.Load();

                // Initialize the Nancy web server module.
                _webServer = new StumpsWebServer(_host, this.Configuration.WebApiPort);

                // Start the host and the web server
                _host.Start();
                _webServer.Start();
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
                    this.Shutdown();
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
