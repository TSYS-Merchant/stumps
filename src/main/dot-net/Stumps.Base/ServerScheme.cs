namespace Stumps
{

    /// <summary>
    ///     The scheme used to listen for connections by the HTTP server.
    /// </summary>
    public enum ServerScheme
    {

        /// <summary>
        /// The server is accessed through HTTP.
        /// </summary>
        Http = 0,

        /// <summary>
        /// The server is accessed through SSL-encrypted HTTP.
        /// </summary>
        Https = 1

    }

}
