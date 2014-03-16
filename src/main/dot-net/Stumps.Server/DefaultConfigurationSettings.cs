namespace Stumps.Server
{

    using System;
    using System.IO;

    /// <summary>
    ///     A class that represents the default configuration settings used by the Stumps server.
    /// </summary>
    public static class DefaultConfigurationSettings
    {

        /// <summary>
        ///     Gets the name of the configuration file.
        /// </summary>
        /// <value>
        ///     The name of the configuration file.
        /// </value>
        public static string ConfigurationFileName
        {
            get { return "stumps.config"; }
        }

        /// <summary>
        ///     Gets the data compatibility version.
        /// </summary>
        /// <value>
        ///     The data compatibility version.
        /// </value>
        public static int DataCompatibilityVersion
        {
            get { return 1; }
        }

        /// <summary>
        ///     Gets the name of the parent storage directory.
        /// </summary>
        /// <value>
        ///     The name of the parent storage directory.
        /// </value>
        public static string StorageDirectoryName
        {
            get { return "Stumps"; }
        }

        /// <summary>
        ///     Gets the default storage path.
        /// </summary>
        /// <value>
        ///     The default storage path.
        /// </value>
        public static string StoragePath
        {
            get
            {
                var dataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                dataPath = Path.Combine(dataPath, DefaultConfigurationSettings.StorageDirectoryName);
                return dataPath;
            }
        }

        /// <summary>
        ///     Gets or sets the default TCP port used to access Rest API and user interface.
        /// </summary>
        /// <value>
        ///     The default TCP port on the local machine used to access the Rest API and user interface.
        /// </value>
        public static int WebApiPort
        {
            get { return 8888; }
        }

    }

}