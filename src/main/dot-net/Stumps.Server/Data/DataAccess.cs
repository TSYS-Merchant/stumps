namespace Stumps.Server.Data
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Stumps.Server.Utility;

    /// <summary>
    ///     A class that provides an implementation of <see cref="T:Stumps.Server.Data.IDataAccess"/>
    ///     that uses JSON files and directory structures to persist information about proxies and stumps.
    /// </summary>
    public sealed class DataAccess : IDataAccess
    {

        /// <summary>
        ///     The file extension used to for files that contain the body matched against an HTTP request.
        /// </summary>
        public const string BodyMatchFileExtension = ".body.match";

        /// <summary>
        ///     The file extension used for files that contain the body used to in response to an HTTP request.
        /// </summary>
        public const string BodyResponseFileExtension = ".body.response";

        /// <summary>
        ///     The file extension used for proxy server configuration files. 
        /// </summary>
        public const string ProxyFileExtension = ".server";

        /// <summary>
        ///     The path used to persist recordings for a proxy server.
        /// </summary>
        public const string RecordingPathName = "recordings";

        /// <summary>
        ///     The file extension used for stumps configuration files. 
        /// </summary>
        public const string StumpFileExtension = ".stump";

        /// <summary>
        ///     The path used to persist stumps for a proxy server.
        /// </summary>
        public const string StumpsPathName = "stumps";

        private readonly string _storagePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Data.DataAccess"/> class.
        /// </summary>
        /// <param name="storagePath">The data path.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="storagePath"/> is <c>null</c>.</exception>
        public DataAccess(string storagePath)
        {

            if (storagePath == null)
            {
                throw new ArgumentNullException("storagePath");
            }

            _storagePath = storagePath;

        }

        /// <summary>
        ///     Gets or sets the path used to access the data store.
        /// </summary>
        /// <value>
        ///     The path used to access the data store.
        /// </value>
        public string StoragePath
        {
            get { return _storagePath; }
        }

        /// <summary>
        ///     Creates an entry for a new proxy server.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> to create.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="server"/> is <c>null</c>.</exception>
        public void ProxyServerCreate(ProxyServerEntity server)
        {

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            var proxyFile = Path.Combine(_storagePath, server.ProxyId + DataAccess.ProxyFileExtension);
            JsonUtility.SerializeToFile(server, proxyFile);

            Directory.CreateDirectory(Path.Combine(_storagePath, server.ProxyId));
            Directory.CreateDirectory(Path.Combine(_storagePath, server.ProxyId, DataAccess.RecordingPathName));
            Directory.CreateDirectory(Path.Combine(_storagePath, server.ProxyId, DataAccess.StumpsPathName));

        }

        /// <summary>
        ///     Deletes an existing <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> to delete.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyId"/> is <c>null</c>.</exception>
        public void ProxyServerDelete(string proxyId)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            var proxyFile = Path.Combine(_storagePath, proxyId + DataAccess.ProxyFileExtension);
            File.Delete(proxyFile);

            var proxyDirectory = Path.Combine(_storagePath, proxyId);
            Directory.Delete(proxyDirectory, true);

        }

        /// <summary>
        ///     Finds the persisted <see cref="T:Stumps.Server.Data.ProxyServerEntity" /> for a specified <paramref name="proxyId"/>.
        /// </summary>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <returns></returns>
        public ProxyServerEntity ProxyServerFind(string proxyId)
        {

            var path = Path.Combine(_storagePath, proxyId + DataAccess.ProxyFileExtension);
            var proxy = JsonUtility.DeserializeFromFile<ProxyServerEntity>(path);
            return proxy;

        }

        /// <summary>
        ///     Finds a list of all persisted <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.Data.ProxyServerEntity" />.
        /// </returns>
        public IList<ProxyServerEntity> ProxyServerFindAll()
        {

            var proxies = JsonUtility.DeserializeFromDirectory<ProxyServerEntity>(
                _storagePath, "*" + DataAccess.ProxyFileExtension, SearchOption.TopDirectoryOnly);

            return proxies;

        }

        /// <summary>
        ///     Loads the contents of a resource for a proxy server.
        /// </summary>
        /// <param name="proxyId">The proxy unique identifier.</param>
        /// <param name="resourceFileName">Name of the file.</param>
        /// <returns>A byte array containing the contents of the resource file.</returns>
        /// <remarks>A <c>null</c> value is returned if the resource file cannot be found.</remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="proxyId"/> is <c>null</c>.
        /// </exception>
        public byte[] ProxyServerReadResource(string proxyId, string resourceFileName)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            resourceFileName = resourceFileName ?? string.Empty;

            byte[] fileBytes = null;

            var path = Path.Combine(_storagePath, proxyId, DataAccess.StumpsPathName, resourceFileName);
            if (File.Exists(path))
            {
                fileBytes = File.ReadAllBytes(path);
            }

            return fileBytes;

        }

        /// <summary>
        ///     Creates a new <see cref="T:Stumps.Server.Data.StumpEntity" /> for an existing proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.StumpEntity" /> to persist.</param>
        /// <param name="matchBody">The array of bytes representing the HTTP body matched against in the stump.</param>
        /// <param name="responseBody">The array of bytes returned as the HTTP body in response to the stump.</param>
        /// <returns>
        /// The created <see cref="T:Stumps.Server.Data.StumpEntity" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="proxyId"/> is <c>null</c>.
        /// or
        /// <paramref name="entity"/> is <c>null</c>.
        /// </exception>
        public StumpEntity StumpCreate(string proxyId, StumpEntity entity, byte[] matchBody, byte[] responseBody)
        {

            if (string.IsNullOrWhiteSpace(proxyId))
            {
                throw new ArgumentNullException("proxyId");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var stumpsPath = Path.Combine(_storagePath, proxyId, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.StumpFileExtension);
            var matchFileName = entity.StumpId + DataAccess.BodyMatchFileExtension;
            var responseFileName = entity.StumpId + DataAccess.BodyResponseFileExtension;

            if (matchBody != null && matchBody.Length > 0)
            {
                entity.Request.BodyFileName = matchFileName;

                var file = Path.Combine(stumpsPath, matchFileName);
                File.WriteAllBytes(file, matchBody);
            }

            if (responseBody != null && responseBody.Length > 0)
            {
                entity.Response.BodyFileName = responseFileName;

                var file = Path.Combine(stumpsPath, responseFileName);
                File.WriteAllBytes(file, responseBody);
            }

            JsonUtility.SerializeToFile(entity, stumpFileName);

            return entity;

        }

        /// <summary>
        ///     Deletes an existing stump from a proxy server.
        /// </summary>
        /// <param name="proxyId">The unique identifier of the proxy the stump is located in.</param>
        /// <param name="stumpId">The  unique identifier for the stump to delete.</param>
        public void StumpDelete(string proxyId, string stumpId)
        {
            var stumpsPath = Path.Combine(_storagePath, proxyId, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, stumpId + DataAccess.StumpFileExtension);
            var matchFileName = Path.Combine(stumpsPath, stumpId + DataAccess.BodyMatchFileExtension);
            var responseFileName = Path.Combine(stumpsPath, stumpId + DataAccess.BodyResponseFileExtension);

            if (File.Exists(stumpFileName))
            {
                File.Delete(stumpFileName);
            }

            if (File.Exists(matchFileName))
            {
                File.Delete(matchFileName);
            }

            if (File.Exists(responseFileName))
            {
                File.Delete(responseFileName);
            }

        }

        /// <summary>
        ///     Finds all a list of all <see cref="T:Stumps.Server.Data.StumpEntity" /> for the specified <paramref name="proxyId" />.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy server.</param>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.Data.StumpEntity" />.
        /// </returns>
        public IList<StumpEntity> StumpFindAll(string proxyId)
        {
            var stumpsPath = Path.Combine(_storagePath, proxyId, DataAccess.StumpsPathName);

            var stumpEntities = JsonUtility.DeserializeFromDirectory<StumpEntity>(
                stumpsPath, "*" + DataAccess.StumpFileExtension, SearchOption.TopDirectoryOnly);

            return stumpEntities;

        }

    }

}