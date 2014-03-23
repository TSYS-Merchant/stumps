namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents access to a data store used to persist information about proxy servers and stumps.
    /// </summary>
    public interface IDataAccess
    {

        /// <summary>
        ///     Creates an entry for a new proxy server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> to create.</param>
        void ProxyServerCreate(ProxyServerEntity server);

        /// <summary>
        ///     Deletes an existing <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> to delete.</param>
        void ProxyServerDelete(string proxyId);

        /// <summary>
        ///     Finds the persisted <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> for a specified <paramref name="proxyId"/>.
        /// </summary>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <returns></returns>
        ProxyServerEntity ProxyServerFind(string proxyId);

        /// <summary>
        ///     Finds a list of all persisted <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.
        /// </summary>
        /// <returns>A generic list of <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.</returns>
        IList<ProxyServerEntity> ProxyServerFindAll();

        /// <summary>
        ///     Creates a new <see cref="T:Stumps.Server.Data.StumpEntity"/> for an existing proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.StumpEntity"/> to persist.</param>
        /// <param name="matchBody">The array of bytes representing the HTTP body matched against in the stump.</param>
        /// <param name="responseBody">The array of bytes returned as the HTTP body in response to the stump.</param>
        /// <returns>The created <see cref="T:Stumps.Server.Data.StumpEntity"/>.</returns>
        StumpEntity StumpCreate(string proxyId, StumpEntity entity, byte[] matchBody, byte[] responseBody);

        /// <summary>
        ///     Deletes an existing stump from a proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier of the proxy the stump is located in.</param>
        /// <param name="stumpId">The  unique identifier for the stump to delete.</param>
        void StumpDelete(string proxyId, string stumpId);

        /// <summary>
        ///     Finds all a list of all <see cref="T:Stumps.Server.Data.StumpEntity"/> for the specified <paramref name="proxyId"/>.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <returns>A generic list of <see cref="T:Stumps.Server.Data.StumpEntity"/>.</returns>
        IList<StumpEntity> StumpFindAll(string proxyId);

    }

}