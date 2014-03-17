namespace Stumps.Server
{

    /// <summary>
    ///     A class that represents an HTTP header.
    /// </summary>
    public sealed class HttpHeader
    {

        /// <summary>
        ///     Gets or sets the name of the HTTP header.
        /// </summary>
        /// <value>
        ///     The name of the HTTP header.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value of the HTTP header.
        /// </summary>
        /// <value>
        ///     The value of the HTTP header.
        /// </value>
        public string Value { get; set; }

    }

}