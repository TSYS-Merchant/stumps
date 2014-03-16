namespace Stumps.Server.Proxy
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents a multitenant proxy server host.
    /// </summary>
    public interface IProxyHost : IDisposable
    {

        /// <summary>
        ///     Creates a new proxy server. 
        /// </summary>
        /// <param name="externalHostName">The name of the external host served by the proxy.</param>
        /// <param name="port">The TCP used to listen for incomming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the external host requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.ProxyEnvironment"/> represeting the new proxy server.
        /// </returns>
        ProxyEnvironment CreateProxy(string externalHostName, int port, bool useSsl, bool autoStart);

        /// <summary>
        /// Deletes an existing proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy.</param>
        void DeleteProxy(string proxyId);

        /// <summary>
        /// Finds all proxy servers represented by the instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.Proxy.ProxyEnvironment"/> objects.
        /// </returns>
        IList<ProxyEnvironment> FindAll();

        /// <summary>
        /// Finds the proxy server with the specified identifier.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.ProxyEnvironment" /> with the specified identifier.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a proxy with the specified <paramref name="proxyId"/>
        ///     is not found.
        /// </remarks>
        ProxyEnvironment FindProxy(string proxyId);

        /// <summary>
        ///     Loads all proxy servers from the data store.
        /// </summary>
        void Load();

        /// <summary>
        ///     Shuts down this instance and all started proxy servers.
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     Shut down the specified proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        void Shutdown(string proxyId);

        /// <summary>
        ///     Starts all proxy servers that are not currently running.
        /// </summary>
        void Start();

        /// <summary>
        ///     Starts the proxy server with the specified unique identifier.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        void Start(string proxyId);

    }

}