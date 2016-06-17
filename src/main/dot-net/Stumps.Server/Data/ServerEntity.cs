namespace Stumps.Server.Data
{

    /// <summary>
    ///     A class that represents the persisted form Stumps Server's configuration.
    /// </summary>
    public sealed class ServerEntity
    {

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically start the Stumps server after it is loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Stumps server should automatically start after it is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to disable stumps when recording.
        /// </summary>
        /// <value>
        /// <c>true</c> to disable stumps when recording; otherwise, <c>false</c>.
        /// </value>
        public bool DisableStumpsWhenRecording { get; set; }

        /// <summary>
        ///     Gets or sets the TCP port the Stumps server listens to for traffic.
        /// </summary>
        /// <value>
        ///     The TCP port the Stumps server listens to for traffic.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        ///     Gets or sets the name of the server.
        /// </summary>
        /// <value>
        ///     The name of the server.
        /// </value>
        public string ServerName { get; set; }

        /// <summary>
        ///     Gets or sets the host name for the remote server.
        /// </summary>
        /// <value>
        ///     The host name of the remote server.
        /// </value>
        public string RemoteServerHostName { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use HTTPS for incomming connections rather than HTTP.
        /// </summary>
        /// <value>
        ///     <c>true</c> to use HTTPS for incomming HTTP connections rather than HTTP.
        /// </value>
        public bool UseHttpsForIncommingConnections { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote server requires an SSL connection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the remote server requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }

    }

}