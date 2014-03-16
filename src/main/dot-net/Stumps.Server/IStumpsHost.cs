namespace Stumps.Server
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents a multitenant proxy server host.
    /// </summary>
    public interface IStumpsHost : IDisposable
    {

        /// <summary>
        ///     Creates a new instance of a Stumps server.
        /// </summary>
        /// <param name="externalHostName">The name of the external host served by the proxy.</param>
        /// <param name="port">The TCP used to listen for incomming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the external host requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the proxy server.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.StumpsServerInstance"/> represeting the new proxy server.
        /// </returns>
        StumpsServerInstance CreateServerInstance(string externalHostName, int port, bool useSsl, bool autoStart);

        /// <summary>
        ///     Deletes an existing proxy server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        void DeleteServerInstance(string serverId);

        /// <summary>
        ///     Finds all Stumps servers hosted by the current instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.StumpsServerInstance"/> objects.
        /// </returns>
        IList<StumpsServerInstance> FindAll();

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
        StumpsServerInstance FindServer(string serverId);

        /// <summary>
        ///     Loads all servers from the data store.
        /// </summary>
        void Load();

        /// <summary>
        ///     Shuts down this instance and all started Stumps servers.
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     Shut down the specified Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        void Shutdown(string serverId);

        /// <summary>
        ///     Starts all Stumps servers that are not currently running.
        /// </summary>
        void Start();

        /// <summary>
        ///     Starts the Stumps server with the specified unique identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        void Start(string serverId);

    }

}