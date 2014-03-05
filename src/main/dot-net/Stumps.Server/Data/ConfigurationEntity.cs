namespace Stumps.Data
{

    /// <summary>
    ///     A class that represents the persisted form of the configuration information.
    /// </summary>
    public class ConfigurationEntity
    {

        /// <summary>
        ///     Gets or sets the data store compatibility version.
        /// </summary>
        /// <value>
        ///     The data store compatibility version.
        /// </value>
        public int DataCompatibilityVersion { get; set; }

        /// <summary>
        ///     Gets or sets the path used to access the data store.
        /// </summary>
        /// <value>
        ///     The path used to access the data store.
        /// </value>
        public string StoragePath { get; set; }

        /// <summary>
        ///     Gets or sets the TCP port used to access Rest API and user interface.
        /// </summary>
        /// <value>
        ///     The TCP port on the local machine used to access the Rest API and user interface.
        /// </value>
        public int WebApiPort { get; set; }

    }

}