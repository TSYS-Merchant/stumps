namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     A class that provides the foundation for persisted HTTP requests and responses. 
    /// </summary>
    public abstract class HttpContextPartEntity
    {

        /// <summary>
        ///     Gets or sets the name of the resource containing the body.
        /// </summary>
        /// <value>
        ///     The name of the resource containing the body.
        /// </value>
        public string BodyResourceName { get; set; }

        /// <summary>
        ///     Gets or sets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Entity is only for persistence.")]
        public IList<NameValuePairEntity> Headers { get; set; }

    }

}
