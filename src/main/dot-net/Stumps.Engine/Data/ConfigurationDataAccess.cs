namespace Stumps.Data {

    using System;
    using System.IO;
    using Stumps.Utility;

    public class ConfigurationDataAccess : IConfigurationDataAccess {

        private readonly string _configurationFile;

        public ConfigurationDataAccess(string configurationFile) {

            if ( configurationFile == null ) {
                throw new ArgumentNullException("configurationFile");
            }

            _configurationFile = configurationFile;

        }

        public ConfigurationEntity LoadConfiguration() {

            var loadedConfiguration = JsonUtility.DeserializeFromFile<ConfigurationEntity>(_configurationFile);
            return loadedConfiguration;

        }

        public void SaveConfiguration(ConfigurationEntity value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            JsonUtility.SerializeToFile(value, _configurationFile);

        }

    }

}
