namespace Stumps
{

    /// <summary>
    ///     Provides the default response for a <see cref="T:Stumps.StumpsServer"/> when a proxy 
    ///     is not defined unavailable, and a <see cref="T:Stumps.Stump"/> was not found matching an
    ///     incomming request. 
    /// </summary>
    public enum ServerDefaultResponse
    {

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
