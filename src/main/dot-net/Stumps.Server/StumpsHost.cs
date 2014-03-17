namespace Stumps.Server
{

    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Stumps.Server.Data;
    using Stumps.Server.Logging;
    using Stumps.Server.Utility;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents a multitenant host of proxy servers.
    /// </summary>
    public class StumpsHost : IStumpsHost
    {

        private readonly IDataAccess _dataAccess;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, StumpsServerInstance> _serverInstances;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.StumpsHost"/> class.
        /// </summary>
        /// <param name="logger">The logger used by the instance.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="logger"/> is <c>null</c>.
        /// or
        /// <paramref name="dataAccess"/> is <c>null</c>.
        /// </exception>
        public StumpsHost(ILogger logger, IDataAccess dataAccess)
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

            _serverInstances = new ConcurrentDictionary<string, StumpsServerInstance>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Server.StumpsHost"/> class.
        /// </summary>
        ~StumpsHost()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Creates a new instance of a Stumps server.
        /// </summary>
        /// <param name="externalHostName">The name of the external host served by the proxy.</param>
        /// <param name="port">The TCP port used to listen for incomming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the external host requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.StumpsServerInstance" /> represeting the new proxy server.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="externalHostName"/> is null.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        /// <exception cref="StumpsNetworkException">The port is already in use.</exception>
        public StumpsServerInstance CreateServerInstance(string externalHostName, int port, bool useSsl, bool autoStart)
        {

            if (string.IsNullOrWhiteSpace(externalHostName))
            {
                throw new ArgumentNullException("externalHostName");
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException("port");
            }

            if (NetworkInformation.IsPortBeingUsed(port))
            {
                throw new StumpsNetworkException(Resources.PortIsInUseError);
            }

            // Create a new data entity representing the Stumps server
            var proxyEntity = new ProxyServerEntity
            {
                AutoStart = autoStart,
                ExternalHostName = externalHostName,
                Port = port,
                UseSsl = useSsl,
                ProxyId = RandomGenerator.GenerateIdentifier()
            };

            // Persist the server data entity
            _dataAccess.ProxyServerCreate(proxyEntity);

            // Create a new service instance from the entity
            UnwrapAndRegisterServer(proxyEntity);

            var server = _serverInstances[proxyEntity.ProxyId];

            if (autoStart)
            {
                server.Start();
            }

            return server;

        }

        /// <summary>
        ///     Deletes an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void DeleteServerInstance(string serverId)
        {

            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException("serverId");
            }

            if (_serverInstances.ContainsKey(serverId))
            {
                _serverInstances[serverId].Stop();
                _serverInstances[serverId].Dispose();

                StumpsServerInstance server;
                _serverInstances.TryRemove(serverId, out server);

                _dataAccess.ProxyServerDelete(serverId);
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
        ///     Finds all Stumps servers hosted by the current instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.StumpsServerInstance"/> objects.
        /// </returns>
        public IList<StumpsServerInstance> FindAll()
        {

            var pairs = _serverInstances.ToArray();

            return pairs.Select(pair => pair.Value).ToList();

        }

        /// <summary>
        ///     Finds the proxy server with the specified identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.StumpsServerInstance" /> with the specified identifier.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a proxy with the specified <paramref name="serverId"/>
        ///     is not found.
        /// </remarks>
        public StumpsServerInstance FindServer(string serverId)
        {

            StumpsServerInstance server;

            _serverInstances.TryGetValue(serverId, out server);

            return server;

        }

        /// <summary>
        ///     Loads all Stumps servers from the data store.
        /// </summary>
        public void Load()
        {

            var proxyEntities = _dataAccess.ProxyServerFindAll();

            foreach (var proxyEntity in proxyEntities)
            {
                UnwrapAndRegisterServer(proxyEntity);
            }

        }

        /// <summary>
        ///     Shuts down this instance and all started Stumps servers.
        /// </summary>
        public void Shutdown()
        {

            foreach (var keyPair in _serverInstances)
            {
                keyPair.Value.Stop();
            }

        }

        /// <summary>
        ///     Shut down the specified Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void Shutdown(string serverId)
        {

            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException("serverId");
            }

            StumpsServerInstance server;
            _serverInstances.TryGetValue(serverId, out server);

            if (server != null)
            {
                server.Stop();
            }

        }

        /// <summary>
        ///     Starts all Stumps servers that are not currently running.
        /// </summary>
        public void Start()
        {

            foreach (var server in _serverInstances)
            {
                if (server.Value.AutoStart)
                {
                    server.Value.Start();
                }
            }

        }

        /// <summary>
        ///     Starts the Stumps server with the specified unique identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void Start(string serverId)
        {

            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException("serverId");
            }

            StumpsServerInstance server;
            _serverInstances.TryGetValue(serverId, out server);

            if (server != null)
            {
                server.Start();
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

                foreach (var keyPair in _serverInstances)
                {
                    if (keyPair.Value != null)
                    {
                        keyPair.Value.Dispose();
                    }
                }

                _serverInstances.Clear();

            }

        }

        /// <summary>
        ///     Creates a new proxy server from a <see cref="T:Stumps.Server.Data.ProxyServerEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.ProxyServerEntity"/> used to create the Stumps server.</param>
        private void UnwrapAndRegisterServer(ProxyServerEntity entity)
        {

            var server = new StumpsServerInstance(entity.ProxyId, _dataAccess)
            {
                ListeningPort = entity.Port,
                UseSsl = entity.UseSsl,
                AutoStart = entity.AutoStart,
                ExternalHostName = entity.ExternalHostName
            };

            _serverInstances.AddOrUpdate(server.ServerId, server, (key, oldServer) => server);

        }

    }

}