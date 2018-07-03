namespace Stumps.Web.Models
{
    /// <summary>
    ///     A class that represents the model for an HTTP header.
    /// </summary>
    public class HeaderModel
    {
        /// <summary>
        ///     Gets or sets the name of the HTTP header.
        /// </summary>
        /// <value>
        /// The name of the HTTP header.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value of the HTTP header.
        /// </summary>
        /// <value>
        /// The value of the HTTP header.
        /// </value>
        public string Value { get; set; }
    }
}