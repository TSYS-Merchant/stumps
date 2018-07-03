namespace Stumps.Web.Models
{
    /// <summary>
    /// A class that represents a Stump.
    /// </summary>
    public class StumpModel
    {
        /// <summary>
        ///     Gets or sets the name of the Stump.
        /// </summary>
        /// <value>
        ///     The name of the Stump.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the origin of the Stump.
        /// </summary>
        /// <value>
        ///     The origin of the Stump.
        /// </value>
        public StumpOrigin Origin { get; set; }

        /// <summary>
        ///     Gets or sets the index of the recorded HTTP request used to create the Stump.
        /// </summary>
        /// <value>
        ///     The index of the recorded HTTP request used to create the Stump.
        /// </value>
        public int RecordId { get; set; }

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
        ///     Gets or sets the request body matching method.
        /// </summary>
        /// <value>
        ///     The request body matching method.
        /// </value>
        public BodyMatch RequestBodyMatch { get; set; }

        /// <summary>
        ///     Gets or sets the request body text matching values.
        /// </summary>
        /// <value>
        ///     The request body text matching values.
        /// </value>
        public string[] RequestBodyMatchValues { get; set; }

        /// <summary>
        ///     Gets or sets the URL used to retrieve the request body.
        /// </summary>
        /// <value>
        ///     The URL used to retrieve the request body.
        /// </value>
        public string RequestBodyUrl { get; set; }

        /// <summary>
        ///     Gets or sets the request matching headers.
        /// </summary>
        /// <value>
        ///     The request matching headers.
        /// </value>
        public HeaderModel[] RequestHeaderMatch { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP method used for the request.
        /// </summary>
        /// <value>
        ///     The HTTP method used for the request.
        /// </value>
        public string RequestHttpMethod { get; set; }

        /// <summary>
        ///     Gets or sets the matching HTTP method required for the Stump.
        /// </summary>
        /// <value>
        ///     The matching HTTP method required for the Stump.
        /// </value>
        public bool RequestHttpMethodMatch { get; set; }

        /// <summary>
        ///     Gets or sets the request raw URL used when making the request.
        /// </summary>
        /// <value>
        ///     The raw URL used when making the request.
        /// </value>
        public string RequestUrl { get; set; }

        /// <summary>
        ///     Gets or sets the matching URL required for the Stump.
        /// </summary>
        /// <value>
        ///     The matching URL required for the Stump.
        /// </value>
        public bool RequestUrlMatch { get; set; }

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
        ///     Gets or sets modified form of the response body.
        /// </summary>
        /// <value>
        ///     The modified form of the response body.
        /// </value>
        public string ResponseBodyModification { get; set; }

        /// <summary>
        ///     Gets or sets the source of the response body.
        /// </summary>
        /// <value>
        ///     The source of the response body.
        /// </value>
        public BodySource ResponseBodySource { get; set; }

        /// <summary>
        ///     Gets or sets URL to the response body.
        /// </summary>
        /// <value>
        ///     The URL to the response body.
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

        /// <summary>
        ///     Gets or sets the unique identifier for the Stump.
        /// </summary>
        /// <value>
        ///     The unique identifier for the Stump.
        /// </value>
        public string StumpId { get; set; }
    }
}