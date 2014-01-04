namespace Stumps.Data {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using Stumps.Utility;

    public sealed class DataAccess : IDataAccess {

        public const string BodyMatchFileExtension = ".body.match";
        public const string BodyResponseFileExtension = ".body.response";
        public const string ProxyFileExtension = ".proxy";
        public const string RecordingPathName = "recordings";
        public const string RootPathName = "Stumps";
        public const string StumpFileExtension = ".stump";
        public const string StumpsPathName = "stumps";

        private readonly string _dataPath;

        public DataAccess() {

            var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            dataPath = Path.Combine(dataPath, DataAccess.RootPathName);

            if ( !Directory.Exists(dataPath) ) {
                Directory.CreateDirectory(dataPath);
            }

            _dataPath = dataPath;

        }

        public void ProxyServerCreate(ProxyServerEntity server) {

            if ( server == null ) {
                throw new ArgumentNullException("server");
            }

            var externalHostName = cleanExternalHostName(server.ExternalHostName);

            var proxyFile = Path.Combine(_dataPath, externalHostName + DataAccess.ProxyFileExtension);
            JsonUtility.SerializeToFile(server, proxyFile);

            Directory.CreateDirectory(Path.Combine(_dataPath, externalHostName));
            Directory.CreateDirectory(Path.Combine(_dataPath, externalHostName, DataAccess.RecordingPathName));
            Directory.CreateDirectory(Path.Combine(_dataPath, externalHostName, DataAccess.StumpsPathName));

        }

        public void ProxyServerDelete(string externalHostName) {

            externalHostName = cleanExternalHostName(externalHostName);

            var proxyFile = Path.Combine(_dataPath, externalHostName + DataAccess.ProxyFileExtension);
            File.Delete(proxyFile);

            var proxyDirectory = Path.Combine(_dataPath, externalHostName);
            Directory.Delete(proxyDirectory, true);

        }

        public IList<ProxyServerEntity> ProxyServerFindAll() {

            var proxies = JsonUtility.DeserializeFromDirectory<ProxyServerEntity>(
                _dataPath, "*" + DataAccess.ProxyFileExtension, SearchOption.TopDirectoryOnly);

            return proxies;

        }

        public StumpEntity StumpCreate(string externalHostName, StumpEntity entity, byte[] matchBody, byte[] responseBody) {

            if ( entity == null ) {
                throw new ArgumentNullException("entity");
            }

            externalHostName = cleanExternalHostName(externalHostName);
            var stumpsPath = Path.Combine(_dataPath, externalHostName, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.StumpFileExtension);
            var matchFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.BodyMatchFileExtension);
            var responseFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.BodyResponseFileExtension);

            if ( matchBody != null && matchBody.Length > 0 ) {
                entity.MatchBodyFileName = matchFileName;
                File.WriteAllBytes(matchFileName, matchBody);
            }

            if ( responseBody != null && responseBody.Length > 0 ) {
                entity.ResponseBodyFileName = responseFileName;
                File.WriteAllBytes(responseFileName, responseBody);
            }

            JsonUtility.SerializeToFile(entity, stumpFileName);

            return entity;

        }

        public void StumpDelete(string externalHostName, string stumpId) {

            externalHostName = cleanExternalHostName(externalHostName);
            var stumpsPath = Path.Combine(_dataPath, externalHostName, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, stumpId + DataAccess.StumpFileExtension);
            var matchFileName = Path.Combine(stumpsPath, stumpId + DataAccess.BodyMatchFileExtension);
            var responseFileName = Path.Combine(stumpsPath, stumpId + DataAccess.BodyResponseFileExtension);

            if ( File.Exists(stumpFileName) ) {
                File.Delete(stumpFileName);
            }

            if ( File.Exists(matchFileName) ) {
                File.Delete(matchFileName);
            }

            if ( File.Exists(responseFileName) ) {
                File.Delete(responseFileName);
            }

        }

        public IList<StumpEntity> StumpFindAll(string externalHostName) {

            externalHostName = cleanExternalHostName(externalHostName);
            var stumpsPath = Path.Combine(_dataPath, externalHostName, DataAccess.StumpsPathName);

            var stumpEntities = JsonUtility.DeserializeFromDirectory<StumpEntity>(
                stumpsPath, "*" + DataAccess.StumpFileExtension, SearchOption.TopDirectoryOnly);

            return stumpEntities;

        }

        private static string cleanExternalHostName(string externalHostName) {
            externalHostName = externalHostName.Replace(':', '.');
            return externalHostName;
        }

    }

}
