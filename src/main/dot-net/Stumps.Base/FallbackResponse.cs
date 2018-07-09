namespace Stumps
{
    /// <summary>
    ///     Provides the fallback response for a <see cref="StumpsServer"/> when a remote host 
    ///     is not defined unavailable, and a <see cref="Stump"/> was not found matching an
    ///     incoming request. 
    /// </summary>
    public enum FallbackResponse
    {
        /// <summary>
        ///     The fallback response is undefined, and will default to an HTTP 503 Service Unavailable response.
        /// </summary>
        Undefined = 0,

        /// <summary>
        ///     Respond with an HTTP 404 Not Found response.
        /// </summary>
        Http404NotFound = 404,

        /// <summary>
        ///     Respond with an HTTP 503 Service Unavailable response.
        /// </summary>
        Http503ServiceUnavailable = 503
    }
}
