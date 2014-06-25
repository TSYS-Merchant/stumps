namespace Stumps.Web.Models
{

    using System;

    /// <summary>
    ///     A class that represents the information about a recorded HTTP request and response.
    /// </summary>
    public class RecordingModel
    {

        /// <summary>
        ///     Gets or sets the index of the recorded HTTP request.
        /// </summary>
        /// <value>
        ///     The index of the recorded HTTP request.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets the date and time the request was recorded.
        /// </summary>
        /// <value>
        ///     The date and time the request was recorded.
        /// </value>
        public DateTime Date { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP method used for the request.
        /// </summary>
        /// <value>
        ///     The HTTP method used for the request.
        /// </value>
        public string Method { get; set; }

        /// <summary>
        ///     Gets or sets the request raw URL used when making the request.
        /// </summary>
        /// <value>
        ///     The raw URL used when making the request.
        /// </value>
        public string RawUrl { get; set; }

        /// <summary>
        ///     Gets or sets the size of the request.
        /// </summary>
        /// <value>
        ///     The size of the request.
        /// </value>
        public int RequestSize { get; set; }

        /// <summary>
        ///     Gets or sets the size of the response.
        /// </summary>
        /// <value>
        ///     The size of the response.
        /// </value>
        public int ResponseSize { get; set; }

        /// <summary>
        ///     Gets or sets the response's HTTP status code.
        /// </summary>
        /// <value>
        ///     The response's HTTP status code.
        /// </value>
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description for the HTTP response status code.
        /// </summary>
        /// <value>
        ///     The description for the HTTP response status code.
        /// </value>
        public string StatusDescription { get; set; }

    }

}