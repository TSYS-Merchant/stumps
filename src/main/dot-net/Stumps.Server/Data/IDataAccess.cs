namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents access to a data store used to persist information about Stumps servers and stump instances.
    /// </summary>
    public interface IDataAccess
    {

        /// <summary>
        ///     Creates an entry for a new stumps server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.Server.Data.ServerEntity" /> to create.</param>
        void ServerCreate(ServerEntity server);

        /// <summary>
        ///     Deletes an existing <see cref="T:Stumps.Server.Data.ServerEntity" />.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="T:Stumps.Server.Data.ServerEntity" /> to delete.</param>
        void ServerDelete(string serverId);

        /// <summary>
        ///     Finds the persisted <see cref="T:Stumps.Server.Data.ServerEntity" /> for a specified <paramref name="serverId"/>.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="T:Stumps.Server.Data.ServerEntity" /> to find.</param>
        /// <returns>The <see cref="T:Stumps.Server.Data.ServerEntity"/> with the specified <paramref name="serverId"/>.</returns>
        ServerEntity ServerFind(string serverId);

        /// <summary>
        ///     Finds a list of all persisted <see cref="T:Stumps.Server.Data.ServerEntity" />.
        /// </summary>
        /// <returns>A generic list of <see cref="T:Stumps.Server.Data.ServerEntity" /> objects.</returns>
        IList<ServerEntity> ServerFindAll();

        /// <summary>
        ///     Loads the contents of a resource for a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="resourceName">Name of the file.</param>
        /// <returns>A byte array containing the contents of the resource.</returns>
        /// <remarks>A <c>null</c> value is returned if the resource cannot be found.</remarks>
        byte[] ServerReadResource(string serverId, string resourceName);
 
        /// <summary>
        ///     Creates a new <see cref="T:Stumps.Server.Data.StumpEntity"/> for an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.StumpEntity"/> to persist.</param>
        /// <param name="matchBody">The array of bytes representing the HTTP body matched against in the stump.</param>
        /// <param name="responseBody">The array of bytes returned as the HTTP body in response to the stump.</param>
        /// <returns>The created <see cref="T:Stumps.Server.Data.StumpEntity"/> object.</returns>
        StumpEntity StumpCreate(string serverId, StumpEntity entity, byte[] matchBody, byte[] responseBody);

        /// <summary>
        ///     Deletes an existing stump from a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server the Stump is located in.</param>
        /// <param name="stumpId">The  unique identifier for the stump to delete.</param>
        void StumpDelete(string serverId, string stumpId);

        /// <summary>
        ///     Finds all a list of all <see cref="T:Stumps.Server.Data.StumpEntity"/> for the specified <paramref name="serverId"/>.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>A generic list of <see cref="T:Stumps.Server.Data.StumpEntity"/> objects.</returns>
        IList<StumpEntity> StumpFindAll(string serverId);

    }

}