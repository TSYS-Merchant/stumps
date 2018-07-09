namespace Stumps.Server
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents a multitenant Stumps server host.
    /// </summary>
    public interface IStumpsHost : IDisposable
    {
        /// <summary>
        ///     Creates a new instance of a Stumps server.
        /// </summary>
        /// <param name="remoteServerHostName">The host name for the remote server by the Stumps server.</param>
        /// <param name="port">The TCP used to listen for incoming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the remote server requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the Stumps server.</param>
        /// <returns>
        ///     A <see cref="StumpsServerInstance"/> represeting the new Stumps server.
        /// </returns>
        StumpsServerInstance CreateServerInstance(string remoteServerHostName, int port, bool useSsl, bool autoStart);

        /// <summary>
        ///     Deletes an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        void DeleteServerInstance(string serverId);

        /// <summary>
        ///     Finds all Stumps servers hosted by the current instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="StumpsServerInstance"/> objects.
        /// </returns>
        IList<StumpsServerInstance> FindAll();

        /// <summary>
        ///     Finds the Stumps server with the specified identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>
        ///     A <see cref="StumpsServerInstance" /> with the specified identifier.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stumps server with the specified <paramref name="serverId"/>
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