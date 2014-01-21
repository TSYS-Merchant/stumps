namespace Stumps {

    using System;
    using System.IO;

    public static class DefaultConfigurationSettings {

        public static string ConfigurationFileName {
            get {
                return "stumps.config";
            }
        }

        public static int DataCompatibilityVersion {
            get {
                return 1;
            }
        }

        public static string StorageDirectoryName {
            get {
                return "Stumps";
            }
        }

        public static string StoragePath {
            get {
                var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                dataPath = Path.Combine(dataPath, DefaultConfigurationSettings.StorageDirectoryName);
                return dataPath;
            }
        }

        public static int WebApiPort {
            get {
                return 8888;
            }
        }

    }

}
