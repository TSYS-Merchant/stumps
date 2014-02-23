namespace Stumps.Web.Models
{

    /// <summary>
    ///     A class that represents the model an error that occurred.
    /// </summary>
    public class ErrorModel
    {

        /// <summary>
        ///     Gets or sets the errors that occurred.
        /// </summary>
        /// <value>
        ///     The errors that occured.
        /// </value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Models used for serialization.")]
        public string[] Errors { get; set; }

        /// <summary>
        ///     Gets or sets the error message.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     Gets or sets the full exception for the error that occurred.
        /// </summary>
        /// <value>
        ///     The full exception for the error that occured.
        /// </value>
        public string FullException { get; set; }

    }

}