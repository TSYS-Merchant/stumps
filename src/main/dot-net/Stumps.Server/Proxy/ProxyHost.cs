namespace Stumps.Server.Proxy
{

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using Stumps.Server.Data;
    using Stumps.Server.Logging;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents a multitenant host of proxy servers.
    /// </summary>
    public class ProxyHost : IProxyHost
    {

        private readonly IDataAccess _dataAccess;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, ProxyServer> _proxies;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Proxy.ProxyHost"/> class.
        /// </summary>
        /// <param name="logger">The logger used by the instance.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="logger"/> is <c>null</c>.
        /// or
        /// <paramref name="dataAccess"/> is <c>null</c>.
        /// </exception>
        public ProxyHost(ILogger logger, IDataAccess dataAccess)
        {

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            if (dataAccess == null)
            {
                throw new ArgumentNullException("dataAccess");
            }

            _logger = logger;
            _dataAccess = dataAccess;

            _proxies = new ConcurrentDictionary<string, ProxyServer>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Server.Proxy.ProxyHost"/> class.
        /// </summary>
        ~ProxyHost()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Creates a new proxy server.
        /// </summary>
        /// <param name="externalHostName">The name of the external host served by the proxy.</param>
        /// <param name="port">The TCP used to listen for incomming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the external host requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.ProxyEnvironment" /> represeting the new proxy server.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="externalHostName"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        /// <exception cref="StumpsNetworkException">The port is already in use.</exception>
        public ProxyEnvironment CreateProxy(string externalHostName, int port, bool useSsl, bool autoStart)
        {

            if (string.IsNullOrWhiteSpace(externalHostName))
            {
                throw new ArgumentNullException("externalHostName");
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            if (NetworkUtility.IsPortBeingUsed(port))
            {
                throw new StumpsNetworkException(Resources.PortIsInUseError);
            }

            var proxyEntity = new ProxyServerEntity
            {
                AutoStart = autoStart,
                ExternalHostName = externalHostName,
                Port = port,
                UseSsl = useSsl,
                ProxyId = RandomGenerator.GenerateIdentifier()
            };

            _dataAccess.ProxyServerCreate(proxyEntity);

            UnwrapAndRegisterProxy(proxyEntity);

            var server = _proxies[proxyEntity.ProxyId];

            if (autoStart)
            {
                server.Start();
            }

            return server.Environment;
        }

        /// <summary>
        ///     Deletes an existing proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyId"/> is <c>null</c>.</exception>
        public void DeleteProxy(string proxyId)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            if (_proxies.ContainsKey(proxyId))
            {
                _proxies[proxyId].Stop();
                _proxies[proxyId].Dispose();

                ProxyServer proxyServer;
                _proxies.TryRemove(proxyId, out proxyServer);

                _dataAccess.ProxyServerDelete(proxyId);
            }

        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {

                this.Dispose(true);
                GC.SuppressFinalize(this);

            }

        }

        /// <summary>
        ///     Finds all proxy servers represented by the instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.Proxy.ProxyEnvironment" /> objects.
        /// </returns>
        public IList<ProxyEnvironment> FindAll()
        {

            var environmentList = new List<ProxyEnvironment>();
            var pairs = _proxies.ToArray();

            foreach (var pair in pairs)
            {
                environmentList.Add(pair.Value.Environment);
            }

            return environmentList;

        }

        /// <summary>
        ///     Finds the proxy server with the specified identifier.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.ProxyEnvironment" /> with the specified identifier.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a proxy with the specified <paramref name="proxyId" />
        ///     is not found.
        /// </remarks>
        public ProxyEnvironment FindProxy(string proxyId)
        {

            ProxyEnvironment environment = null;

            ProxyServer server;
            _proxies.TryGetValue(proxyId, out server);

            if (server != null)
            {
                environment = server.Environment;
            }

            return environment;

        }

        /// <summary>
        ///     Loads all proxy servers from the data store.
        /// </summary>
        public void Load()
        {

            var proxyEntities = _dataAccess.ProxyServerFindAll();

            foreach (var proxyEntity in proxyEntities)
            {
                UnwrapAndRegisterProxy(proxyEntity);
            }

        }

        /// <summary>
        ///     Starts all proxy servers that are not currently running.
        /// </summary>
        public void Start()
        {

            foreach (var server in _proxies)
            {
                if (server.Value.Environment.AutoStart)
                {
                    server.Value.Start();
                }
            }

        }

        /// <summary>
        ///     Starts the proxy server with the specified unique identifier.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyId"/> is <c>null</c>.</exception>
        public void Start(string proxyId)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            ProxyServer server;
            _proxies.TryGetValue(proxyId, out server);

            if (server != null)
            {
                server.Start();
            }

        }

        /// <summary>
        ///     Shuts down this instance and all started proxy servers.
        /// </summary>
        public void Shutdown()
        {

            foreach (var keyPair in _proxies)
            {
                keyPair.Value.Stop();
            }

        }

        /// <summary>
        ///     Shut down the specified proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyId"/> is <c>null</c>.</exception>
        public void Shutdown(string proxyId)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            ProxyServer server;
            _proxies.TryGetValue(proxyId, out server);

            if (server != null)
            {
                server.Stop();
            }

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposing && !_disposed)
            {

                _disposed = true;

                foreach (var keyPair in _proxies)
                {
                    if (keyPair.Value != null)
                    {
                        keyPair.Value.Dispose();
                    }
                }

                _proxies.Clear();

            }

        }

        /// <summary>
        ///     Creates a new proxy server from a <see cref="T:Stumps.Server.Data.ProxyServerEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.ProxyServerEntity"/> used to create the proxy server.</param>
        private void UnwrapAndRegisterProxy(ProxyServerEntity entity)
        {

            var environment = new ProxyEnvironment(entity.ProxyId, _dataAccess)
            {
                Port = entity.Port,
                UseSsl = entity.UseSsl,
                AutoStart = entity.AutoStart,
                ExternalHostName = entity.ExternalHostName
            };

            var server = new ProxyServer(environment, _logger);

            environment.Stumps.Load();

            _proxies.AddOrUpdate(environment.ProxyId, server, (key, oldServer) => server);

        }

    }

}