namespace Stumps.Data
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Stumps.Utility;

    public sealed class DataAccess : IDataAccess
    {

        public const string BodyMatchFileExtension = ".body.match";
        public const string BodyResponseFileExtension = ".body.response";
        public const string ProxyFileExtension = ".proxy";
        public const string RecordingPathName = "recordings";
        public const string StumpFileExtension = ".stump";
        public const string StumpsPathName = "stumps";

        private readonly string _dataPath;

        public DataAccess(string dataPath)
        {

            if (dataPath == null)
            {
                throw new ArgumentNullException("dataPath");
            }

            _dataPath = dataPath;

        }

        public string DataPath
        {
            get { return _dataPath; }
        }

        public void ProxyServerCreate(ProxyServerEntity server)
        {

            if (server == null)
            {
                throw new ArgumentNullException("server");
            }

            var proxyFile = Path.Combine(_dataPath, server.ProxyId + DataAccess.ProxyFileExtension);
            JsonUtility.SerializeToFile(server, proxyFile);

            Directory.CreateDirectory(Path.Combine(_dataPath, server.ProxyId));
            Directory.CreateDirectory(Path.Combine(_dataPath, server.ProxyId, DataAccess.RecordingPathName));
            Directory.CreateDirectory(Path.Combine(_dataPath, server.ProxyId, DataAccess.StumpsPathName));

        }

        public void ProxyServerDelete(string proxyId)
        {
            var proxyFile = Path.Combine(_dataPath, proxyId + DataAccess.ProxyFileExtension);
            File.Delete(proxyFile);

            var proxyDirectory = Path.Combine(_dataPath, proxyId);
            Directory.Delete(proxyDirectory, true);

        }

        public IList<ProxyServerEntity> ProxyServerFindAll()
        {

            var proxies = JsonUtility.DeserializeFromDirectory<ProxyServerEntity>(
                _dataPath, "*" + DataAccess.ProxyFileExtension, SearchOption.TopDirectoryOnly);

            return proxies;

        }

        public StumpEntity StumpCreate(string proxyId, StumpEntity entity, byte[] matchBody, byte[] responseBody)
        {

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var stumpsPath = Path.Combine(_dataPath, proxyId, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.StumpFileExtension);
            var matchFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.BodyMatchFileExtension);
            var responseFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.BodyResponseFileExtension);

            if (matchBody != null && matchBody.Length > 0)
            {
                entity.MatchBodyFileName = matchFileName;
                File.WriteAllBytes(matchFileName, matchBody);
            }

            if (responseBody != null && responseBody.Length > 0)
            {
                entity.ResponseBodyFileName = responseFileName;
                File.WriteAllBytes(responseFileName, responseBody);
            }

            JsonUtility.SerializeToFile(entity, stumpFileName);

            return entity;

        }

        public void StumpDelete(string proxyId, string stumpId)
        {
            var stumpsPath = Path.Combine(_dataPath, proxyId, DataAccess.StumpsPathName);

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

        public IList<StumpEntity> StumpFindAll(string proxyId)
        {
            var stumpsPath = Path.Combine(_dataPath, proxyId, DataAccess.StumpsPathName);

            var stumpEntities = JsonUtility.DeserializeFromDirectory<StumpEntity>(
                stumpsPath, "*" + DataAccess.StumpFileExtension, SearchOption.TopDirectoryOnly);

            return stumpEntities;

        }

    }

}