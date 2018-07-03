namespace Stumps.Web.Models
{
    using System;

    /// <summary>
    ///     A class that represents the information about a recorded HTTP request and response.
    /// </summary>
    public class RecordingDetailsModel
    {
        /// <summary>
        ///     Gets or sets the index of the recorded HTTP request.
        /// </summary>
        /// <value>
        ///     The index of the recorded HTTP request.
        /// </value>
        public int Index { get; set; }

        /// <summary>
        ///     Gets or sets the body of the HTTP request.
        /// </summary>
        /// <value>
        ///     The body of the HTTP request.
        /// </value>
        public string RequestBody { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the request body is an image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the request body is an image; otherwise, <c>false</c>.
        /// </value>
        public bool RequestBodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the request body is text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the request body is text; otherwise, <c>false</c>.
        /// </value>
        public bool RequestBodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets the length of the request body.
        /// </summary>
        /// <value>
        ///     The length of the request body.
        /// </value>
        public int RequestBodyLength { get; set; }

        /// <summary>
        ///     Gets or sets the URL used to retrieve the request body.
        /// </summary>
        /// <value>
        ///     The URL used to retrieve the request body.
        /// </value>
        public string RequestBodyUrl { get; set; }

        /// <summary>
        ///     Gets or sets the date and time the request was recorded.
        /// </summary>
        /// <value>
        ///     The date and time the request was recorded.
        /// </value>
        public DateTime RequestDate { get; set; }

        /// <summary>
        ///     Gets or sets the an array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects that represent 
        ///     the HTTP headers sent in the request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.Web.Models.HeaderModel"/> objects headers that represent the HTTP headers sent in the request.
        /// </value>
        public HeaderModel[] RequestHeaders { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP method used for the request.
        /// </summary>
        /// <value>
        ///     The HTTP method used for the request.
        /// </value>
        public string RequestHttpMethod { get; set; }

        /// <summary>
        ///     Gets or sets the request raw URL used when making the request.
        /// </summary>
        /// <value>
        ///     The raw URL used when making the request.
        /// </value>
        public string RequestRawUrl { get; set; }

        /// <summary>
        ///     Gets or sets the body of the HTTP response.
        /// </summary>
        /// <value>
        ///     The body of the HTTP response.
        /// </value>
        public string ResponseBody { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the response body is an image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the response body is an image; otherwise, <c>false</c>.
        /// </value>
        public bool ResponseBodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the response body is text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the request body is text; otherwise, <c>false</c>.
        /// </value>
        public bool ResponseBodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets the length of the response body.
        /// </summary>
        /// <value>
        ///     The length of the response body.
        /// </value>
        public int ResponseBodyLength { get; set; }

        /// <summary>
        ///     Gets or sets the URL used to retrieve the response body.
        /// </summary>
        /// <value>
        ///     The URL used to retrieve the response body.
        /// </value>
        public string ResponseBodyUrl { get; set; }

        /// <summary>
        ///     Gets or sets the an array of <see cref="T:Stumps.Web.Models.HeaderModel"/> objects that represent 
        ///     the HTTP headers sent in the response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.Web.Models.HeaderModel"/> objects headers that represent the HTTP headers 
        ///     sent in the response.
        /// </value>
        public HeaderModel[] ResponseHeaders { get; set; }

        /// <summary>
        ///     Gets or sets the response's HTTP status code.
        /// </summary>
        /// <value>
        ///     The response's HTTP status code.
        /// </value>
        public int ResponseStatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description for the HTTP response status code.
        /// </summary>
        /// <value>
        ///     The description for the HTTP response status code.
        /// </value>
        public string ResponseStatusDescription { get; set; }
    }
}