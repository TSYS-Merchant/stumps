namespace Stumps
{

    using System;
    using Stumps.Http;
    using System.Threading.Tasks;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that provides a fallback
    ///     response to an incomming HTTP request.
    /// </summary>
    internal class FallbackResponseHandler : IHttpHandler
    {

        private readonly int _statusCode;
        private readonly string _statusCodeDescription;
        private readonly HttpResponseOrigin _origin;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.FallbackResponseHandler"/> class.
        /// </summary>
        /// <param name="response">The default response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="response"/> is <c>null</c>.</exception>
        public FallbackResponseHandler(FallbackResponse response)
        {

            if (!Enum.IsDefined(typeof(FallbackResponse), response))
            {
                throw new ArgumentOutOfRangeException("response");
            }

            // Fallback to 503 Service Unavailable when undefined
            response = response == FallbackResponse.Undefined ? FallbackResponse.Http503ServiceUnavailable : response;

            _statusCode = (int)response;
            _statusCodeDescription = HttpStatusCodes.GetStatusDescription(_statusCode);

            _origin = response == FallbackResponse.Http404NotFound
                          ? HttpResponseOrigin.NotFoundResponse
                          : HttpResponseOrigin.ServiceUnavailable;

        }

        /// <summary>
        ///     Occurs when an incomming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        /// A member of the <see cref="T:Stumps.Http.ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Headers.Clear();
            context.Response.ClearBody();
            context.Response.StatusCode = _statusCode;
            context.Response.StatusDescription = _statusCodeDescription;

            var stumpsResponse = context.Response as StumpsHttpResponse;

            if (stumpsResponse != null)
            {
                stumpsResponse.Origin = _origin;
            }

            if (this.ContextProcessed != null)
            {
                this.ContextProcessed(this, new StumpsContextEventArgs(context));
            }

            return ProcessHandlerResult.Terminate;

        }

    }

}
