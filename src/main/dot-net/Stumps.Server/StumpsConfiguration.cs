namespace Stumps.Server
{
    using System;
    using System.IO;
    using System.Net;
    using Stumps.Server.Data;

    /// <summary>
    ///     A class that represents the configuration used for the Stumps server.
    /// </summary>
    public class StumpsConfiguration
    {
        /// <summary>
        /// The maximum data compatibility version number allowed.
        /// </summary>
        public const int MaximumDataCompatibilityVersion = 1000;

        /// <summary>
        /// The minimum data compatibility version number allowed.
        /// </summary>
        public const int MinimumDataCompatibilityVersion = 1;

        private readonly IConfigurationDataAccess _dataAccess;
        private ConfigurationEntity _configurationEntity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsConfiguration"/> class.
        /// </summary>
        /// <param name="dataAccess">The data access.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dataAccess"/> is <c>null</c>.</exception>
        public StumpsConfiguration(IConfigurationDataAccess dataAccess)
        {
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));
            _configurationEntity = CreateDefaultConfigurationEntity();
        }

        /// <summary>
        ///     Gets the data compatibility version.
        /// </summary>
        /// <value>
        ///     The data compatibility version.
        /// </value>
        public int DataCompatibilityVersion
        {
            get => _configurationEntity.DataCompatibilityVersion;
            set => _configurationEntity.DataCompatibilityVersion = value;
        }

        /// <summary>
        ///     Gets the default storage path.
        /// </summary>
        /// <value>
        ///     The default storage path.
        /// </value>
        public string StoragePath
        {
            get => _configurationEntity.StoragePath;
            set => _configurationEntity.StoragePath = value;
        }

        /// <summary>
        ///     Gets or sets the TCP port used to access Rest API and user interface.
        /// </summary>
        /// <value>
        ///     The TCP port on the local machine used to access the Rest API and user interface.
        /// </value>
        public int WebApiPort
        {
            get => _configurationEntity.WebApiPort;
            set => _configurationEntity.WebApiPort = value;
        }

        /// <summary>
        ///     Loads the configuration from the data store.
        /// </summary>
        public void LoadConfiguration() => _configurationEntity = _dataAccess.LoadConfiguration();

        /// <summary>
        ///     Saves the configuration to the data store.
        /// </summary>
        public void SaveConfiguration() => _dataAccess.SaveConfiguration(_configurationEntity);

        /// <summary>
        ///     Validates the configuration settings are correct.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the configuration settings are correct; otherwise, <c>false</c>.
        /// </returns>
        public bool ValidateConfigurationSettings()
        {
            var isConfigurationInvalid = this.WebApiPort < IPEndPoint.MinPort || this.WebApiPort > IPEndPoint.MaxPort ||
                                         this.DataCompatibilityVersion < StumpsConfiguration.MinimumDataCompatibilityVersion ||
                                         this.DataCompatibilityVersion > StumpsConfiguration.MaximumDataCompatibilityVersion ||
                                         !DirectoryIsValid(this.StoragePath);

            return !isConfigurationInvalid;
        }

        /// <summary>
        ///     Creates a default configuration entity.
        /// </summary>
        /// <returns>
        ///     A <see cref="ConfigurationEntity"/> initalized with the default values.
        /// </returns>
        private static ConfigurationEntity CreateDefaultConfigurationEntity()
        {
            var entity = new ConfigurationEntity
            {
                DataCompatibilityVersion = DefaultConfigurationSettings.DataCompatibilityVersion,
                StoragePath = DefaultConfigurationSettings.StoragePath,
                WebApiPort = DefaultConfigurationSettings.WebApiPort
            };

            return entity;
        }

        /// <summary>
        ///     Validates that the given directory is valid and exists.
        /// </summary>
        /// <param name="directoryPath">The path to the directory to validate.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="directoryPath"/> is valid; otherwise, <c>false</c>.
        /// </returns>
        private static bool DirectoryIsValid(string directoryPath)
        {
            bool isValid;

            try
            {
                isValid = Directory.Exists(directoryPath) && Path.IsPathRooted(directoryPath);
            }
            catch (ArgumentException)
            {
                isValid = false;
            }

            return isValid;
        }
    }
}