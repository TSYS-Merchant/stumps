namespace Stumps
{
    using Stumps.Http;

    /// <summary>
    ///     A reference <see cref="IStumpsHttpResponse"/> objects representing common HTTP errors.
    /// </summary>
    public static class HttpErrorResponses
    {
        /// <summary>
        ///     Gets an <see cref="IStumpsHttpResponse" /> representing an HTTP 404 (Not Found) response.
        /// </summary>
        /// <value>
        ///     An <see cref="IStumpsHttpResponse" /> representing an HTTP 404 (Not Found) response.
        /// </value>
        public static IStumpsHttpResponse HttpNotFound
        {
            get
            {
                var response = new BasicHttpResponse
                {
                    StatusCode = HttpStatusCodes.HttpNotFound,
                    StatusDescription = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpNotFound)
                };

                return response;
            }
        }

        /// <summary>
        ///     Gets an <see cref="IStumpsHttpResponse" /> representing an HTTP 500 (Internal Server Error) response.
        /// </summary>
        /// <value>
        ///     An <see cref="IStumpsHttpResponse" /> representing an HTTP 500 (Internal Server Error) response.
        /// </value>
        public static IStumpsHttpResponse HttpInternalServerError
        {
            get
            {
                var respose = new BasicHttpResponse
                {
                    StatusCode = HttpStatusCodes.HttpInternalServerError,
                    StatusDescription = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpInternalServerError)
                };

                return respose;
            }
        }

        /// <summary>
        ///     Gets an <see cref="IStumpsHttpResponse" /> representing an HTTP 501 (Not Implemented) response.
        /// </summary>
        /// <value>
        ///     An <see cref="IStumpsHttpResponse" /> representing an HTTP 501 (Not Implemented) response.
        /// </value>
        public static IStumpsHttpResponse HttpNotImplemented
        {
            get
            {
                var respose = new BasicHttpResponse
                {
                    StatusCode = HttpStatusCodes.HttpNotImplemented,
                    StatusDescription = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpNotImplemented)
                };

                return respose;
            }
        }

        /// <summary>
        ///     Gets an <see cref="IStumpsHttpResponse" /> representing an HTTP 503 (Service Unavailable) response.
        /// </summary>
        /// <value>
        ///     An <see cref="IStumpsHttpResponse" /> representing an HTTP 503 (Service Unavailable) response.
        /// </value>
        public static IStumpsHttpResponse HttpServiceUnavailable
        {
            get
            {
                var respose = new BasicHttpResponse
                {
                    StatusCode = HttpStatusCodes.HttpServiceUnavailable,
                    StatusDescription = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpServiceUnavailable)
                };

                return respose;
            }
        }
    }
}