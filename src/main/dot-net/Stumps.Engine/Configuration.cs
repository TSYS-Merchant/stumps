namespace Stumps {

    using System;
    using System.IO;
    using System.Net;
    using Stumps.Data;

    public class Configuration {

        private readonly IConfigurationDataAccess _dataAccess;
        private ConfigurationEntity _configurationEntity;

        public const int MaximumDataCompatibilityVersion = 1000;

        public const int MinimumDataCompatibilityVersion = 1;

        public Configuration(IConfigurationDataAccess dataAccess) {

            if ( dataAccess == null ) {
                throw new ArgumentNullException("dataAccess");
            }

            _dataAccess = dataAccess;
            _configurationEntity = CreateDefaultConfigurationEntity();

        }

        public int DataCompatibilityVersion {
            get {
                return _configurationEntity.DataCompatibilityVersion;
            }
            set {
                _configurationEntity.DataCompatibilityVersion = value;
            }
        }

        public string StoragePath {
            get {
                return _configurationEntity.StoragePath;
            }
            set {
                _configurationEntity.StoragePath = value;
            }
        }

        public int WebApiPort {
            get {
                return _configurationEntity.WebApiPort;
            }
            set {
                _configurationEntity.WebApiPort = value;
            }
        }

        public void LoadConfiguration() {
            _configurationEntity = _dataAccess.LoadConfiguration();
        }

        public void SaveConfiguration() {
            _dataAccess.SaveConfiguration(_configurationEntity);
        }

        public bool ValidateConfiguration() {

            var isConfigurationInvalid = this.WebApiPort < IPEndPoint.MinPort ||
                                         this.WebApiPort > IPEndPoint.MaxPort ||
                                         this.DataCompatibilityVersion < Configuration.MinimumDataCompatibilityVersion ||
                                         this.DataCompatibilityVersion > Configuration.MaximumDataCompatibilityVersion ||
                                         !DirectoryIsValid(this.StoragePath);

            return !isConfigurationInvalid;

        }

        private static ConfigurationEntity CreateDefaultConfigurationEntity() {

            var entity = new ConfigurationEntity {
                DataCompatibilityVersion = DefaultConfigurationSettings.DataCompatibilityVersion,
                StoragePath = DefaultConfigurationSettings.StoragePath,
                WebApiPort = DefaultConfigurationSettings.WebApiPort
            };

            return entity;

        }

        private static bool DirectoryIsValid(string path) {

            bool isValid;

            try {
                isValid = Directory.Exists(path) && Path.IsPathRooted(path);
            }
            catch (ArgumentException) {
                isValid = false;
            }

            return isValid;

        }

    }

}
