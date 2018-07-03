namespace Stumps.Server.Data
{
    /// <summary>
    ///     A class that represents the persisted form of an object representing a name and value pair.
    /// </summary>
    public class NameValuePairEntity
    {
        /// <summary>
        ///     Gets or sets the name of the HTTP header.
        /// </summary>
        /// <value>
        ///     The name of the HTTP header.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the value of the HTTP header.
        /// </summary>
        /// <value>
        ///     The value of the HTTP header.
        /// </value>
        public string Value
        {
            get;
            set;
        }
    }
}
