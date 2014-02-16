namespace Stumps.Data
{

    /// <summary>
    ///     A class that represents the persisted form proxy server's configuration.
    /// </summary>
    public class StumpEntity
    {

        /// <summary>
        ///     Gets or sets the HTTP method returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The HTTP method returned in response to the stump.
        /// </value>
        public string HttpMethod { get; set; }

        /// <summary>
        ///     Gets or sets the content type in an HTTP request required for the stump to match.
        /// </summary>
        /// <value>
        ///     The content type in an HTTP request required for the stump to match.
        /// </value>
        public string MatchBodyContentType { get; set; }

        /// <summary>
        ///     Gets or sets the path to the file containing the body for an HTTP request required for the stump.
        /// </summary>
        /// <value>
        ///     The path to the file containing the body for an HTTP request required for the stump.
        /// </value>
        public string MatchBodyFileName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether data stored in the <see cref="P:Stumps.Data.StumpEntity.MatchBodyFileName"/> is an image.
        /// </summary>
        /// <value>
        ///     <c>true</c> if data stored in the <see cref="P:Stumps.Data.StumpEntity.MatchBodyFileName"/> is an image; otherwise, <c>false</c>.
        /// </value>
        public bool MatchBodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether data stored in the <see cref="P:Stumps.Data.StumpEntity.MatchBodyFileName"/> is text.
        /// </summary>
        /// <value>
        ///     <c>true</c> if data stored in the <see cref="P:Stumps.Data.StumpEntity.MatchBodyFileName"/> is text; otherwise, <c>false</c>.
        /// </value>
        public bool MatchBodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets maximum length of the body in an HTTP request allowed for the stump to match.
        /// </summary>
        /// <value>
        ///     The minimum length of the body in an HTTP request allowed for the stump to match.
        /// </value>
        public int MatchBodyMaximumLength { get; set; }

        /// <summary>
        ///     Gets or sets minimum length of the body in an HTTP request allowed for the stump to match.
        /// </summary>
        /// <value>
        ///     The minimum length of the body in an HTTP request allowed for the stump to match.
        /// </value>
        public int MatchBodyMinimumLength { get; set; }

        /// <summary>
        ///     Gets or sets an array of formatted <see cref="T:System.String"/> values evaluated against the body in an HTTP request for the stump to match.
        /// </summary>
        /// <value>
        ///     The array of formatted <see cref="T:System.String"/> values evaluated against the body in an HTTP request for the stump to match.
        /// </value>
        public string[] MatchBodyText { get; set; }

        /// <summary>
        ///     Gets or sets an array of formatted <see cref="T:Stumps.Data.HeaderEntity"/> values evaluated against the headers of an HTTP request for the stump to match.
        /// </summary>
        /// <value>
        ///     The array of formatted <see cref="T:Stumps.Data.HeaderEntity"/> values evaluated against the headers of an HTTP request for the stump to match.
        /// </value>
        public HeaderEntity[] MatchHeaders { get; set; }

        /// <summary>
        ///     Gets or sets method of an HTTP request required for the stump to match.
        /// </summary>
        /// <value>
        ///     The method of an HTTP request required for the stump to match.
        /// </value>
        public bool MatchHttpMethod { get; set; }

        /// <summary>
        ///     Gets or sets raw URL of an HTTP request required for the stump to match.
        /// </summary>
        /// <value>
        ///     The raw URL of an HTTP request required for the stump to match.
        /// </value>
        public bool MatchRawUrl { get; set; }

        /// <summary>
        ///     Gets or sets the raw URL returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The raw URL returned in the response to the stump.
        /// </value>
        public string RawUrl { get; set; }

        /// <summary>
        ///     Gets or sets the content type returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The content type returned in the response to the stump.
        /// </value>
        public string ResponseBodyContentType { get; set; }

        /// <summary>
        ///     Gets or sets the file containing the body returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The file containing the body returned in the response to the stump.
        /// </value>
        public string ResponseBodyFileName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether data stored in the <see cref="P:Stumps.Data.StumpEntity.ResponseBodyFileName"/> is an image.
        /// </summary>
        /// <value>
        ///     <c>true</c> if data stored in the <see cref="P:Stumps.Data.StumpEntity.ResponseBodyFileName"/> is an image; otherwise, <c>false</c>.
        /// </value>
        public bool ResponseBodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether data stored in the <see cref="P:Stumps.Data.StumpEntity.ResponseBodyFileName"/> is text.
        /// </summary>
        /// <value>
        ///     <c>true</c> if data stored in the <see cref="P:Stumps.Data.StumpEntity.ResponseBodyFileName"/> is text; otherwise, <c>false</c>.
        /// </value>
        public bool ResponseBodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets the array of <see cref="T:Stumps.Data.HeaderEntity"/> values returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The array of <see cref="T:Stumps.Data.HeaderEntity"/> values returned in the response to the stump.
        /// </value>
        public HeaderEntity[] ResponseHeaders { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The HTTP status code returned in the response to the stump.
        /// </value>
        public int ResponseStatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code returned in the response to the stump.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code returned in the response to the stump.
        /// </value>
        public string ResponseStatusDescription { get; set; }

        /// <summary>
        ///     Gets or sets the organizational category the stump belongs to.
        /// </summary>
        /// <value>
        ///     The organizational category the stump belongs to.
        /// </value>
        public string StumpCategory { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the stump.
        /// </summary>
        /// <value>
        ///     Gets or sets the unique identifier for the stump.
        /// </value>
        public string StumpId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the stump.
        /// </summary>
        /// <value>
        ///     The name of the stump.
        /// </value>
        public string StumpName { get; set; }

    }

}