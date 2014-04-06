namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     A class that provides the foundation for persisted HTTP requests and responses. 
    /// </summary>
    public abstract class HttpContextPartEntity
    {

        /// <summary>
        ///     Gets or sets the name of the file containing the body.
        /// </summary>
        /// <value>
        ///     The name of the file containing the body.
        /// </value>
        public string BodyFileName { get; set; }

        /// <summary>
        ///     Gets or sets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        public List<NameValuePairEntity> Headers { get; set; }

    }

}
