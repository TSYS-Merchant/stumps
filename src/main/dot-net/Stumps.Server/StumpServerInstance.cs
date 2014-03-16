namespace Stumps.Server
{

    using System;
    using Stumps.Server.Data;
    using Stumps.Server.Proxy;

    /// <summary>
    /// A class that represents an the environment and configuration of a proxy server.
    /// </summary>
    public class StumpsServerInstance : IDisposable
    {

        /// <summary>
        ///     The format for a URI for an insecure HTTP connection.
        /// </summary>
        private const string InsecureUriFormat = "http://{0}/";

        /// <summary>
        ///     The format for a URI for a secure HTTP connection.
        /// </summary>
        private const string SecureUriFormat = "https://{0}/";

        private readonly IDataAccess _dataAccess;
        private StumpsServer _server;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.StumpsServerInstance"/> class.
        /// </summary>
        /// <param name="proxyId">The unique identifier of the proxy.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        public StumpsServerInstance(string proxyId, IDataAccess dataAccess)
        {

            this.Recordings = new ProxyRecordings();
            this.Stumps = new ProxyStumps(proxyId, dataAccess);

            _dataAccess = dataAccess;
            this.ServerId = proxyId;
            
            InitializeServer();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Server.StumpsServerInstance"/> class.
        /// </summary>
        ~StumpsServerInstance()
        {
            this.Dispose();
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically start the instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance should automatically; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        ///     Gets or sets the name of the external host.
        /// </summary>
        /// <value>
        ///     The name of the external host.
        /// </value>
        public string ExternalHostName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                var isServerRunning = this._server != null && this._server.IsRunning;
                return isServerRunning;
            }
        }

        /// <summary>
        ///     Gets or sets the port the Stumps server is listening on for incomming HTTP requests.
        /// </summary>
        /// <value>
        ///     The port the Stumps server is listening on for incomming HTTP requests.
        /// </value>
        public int ListeningPort { get; set; }

        /// <summary>
        ///     Gets the recorded HTTP requests and responses.
        /// </summary>
        /// <value>
        ///     The recorded HTTP requests and responses.
        /// </value>
        public ProxyRecordings Recordings { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to record all traffic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if traffic should be recorded; otherwise, <c>false</c>.
        /// </value>
        public bool RecordTraffic { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId { get; set; }

        /// <summary>
        ///     Gets the stumps used by the proxy server.
        /// </summary>
        /// <value>
        ///     The stumps used by the proxy server.
        /// </value>
        public ProxyStumps Stumps { get; private set; }
        
        /// <summary>
        ///     Gets or sets a value indicating whether the exernal host requires SSL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the external host requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }
        
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {
                _disposed = true;

                if (this.IsRunning)
                {
                    this.Stop();
                }

                _server.Dispose();

            }

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Starts this instance of the Stumps server.
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        ///     Stops this instance of the Stumps server.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }

        /// <summary>
        ///     Initializes the Stumps server controlled by this instance.
        /// </summary>
        private void InitializeServer()
        {
            
            // Find the persisted server entity 
            var entity = _dataAccess.ProxyServerFind(this.ServerId);
            this.AutoStart = entity.AutoStart;
            this.ExternalHostName = entity.ExternalHostName;
            this.ListeningPort = entity.Port;
            this.UseSsl = entity.UseSsl;

            if (!string.IsNullOrWhiteSpace(this.ExternalHostName))
            {
                var pattern = this.UseSsl
                                  ? StumpsServerInstance.SecureUriFormat
                                  : StumpsServerInstance.InsecureUriFormat;

                var uriString = string.Format(pattern, this.ExternalHostName);

                var uri = new Uri(uriString);

                _server = new StumpsServer(this.ListeningPort, uri);
            }
            else
            {
                // TODO: Choose which method to use for the fallback when no proxy is available.
                _server = new StumpsServer(this.ListeningPort, ServerDefaultResponse.Http503ServiceUnavailable);
            }

        }
        
    }

}