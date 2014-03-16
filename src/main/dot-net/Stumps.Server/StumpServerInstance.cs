namespace Stumps.Server
{

    using System;
    using System.Threading;
    using Stumps.Server.Data;
    using Stumps.Server.Proxy;

    /// <summary>
    /// A class that represents an the environment and configuration of a proxy server.
    /// </summary>
    public class StumpsServerInstance : IDisposable
    {

        private IDataAccess _dataAccess;
        private int _requestsServed;
        private int _stumpsServed;

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
            this.ProxyId = proxyId;
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
        public bool IsRunning { get; set; }

        /// <summary>
        ///     Gets or sets the port the proxy is listening on for incomming HTTP requests.
        /// </summary>
        /// <value>
        ///     The port the proxy is listening on for incomming HTTP requests.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the proxy.
        /// </summary>
        /// <value>
        ///     The unique identifier for the proxy.
        /// </value>
        public string ProxyId { get; set; }

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
        ///     Gets the stumps used by the proxy server.
        /// </summary>
        /// <value>
        ///     The stumps used by the proxy server.
        /// </value>
        public ProxyStumps Stumps { get; private set; }

        /// <summary>
        ///     Gets the number of Stumps served by the instance.
        /// </summary>
        /// <value>
        ///     The number of Stumps served by the instance.
        /// </value>
        public int StumpsServed
        {
            get { return _stumpsServed; }
        }

        /// <summary>
        ///     Gets the total number of requests served by the instance.
        /// </summary>
        /// <value>
        ///     The total number of requests served by the instance.
        /// </value>
        public int RequestsServed
        {
            get { return _requestsServed; }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the exernal host requires SSL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the external host requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void Dispose()
        {
        }


    }

}