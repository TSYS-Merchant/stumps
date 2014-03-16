namespace Stumps
{

    using System;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that provides a default
    ///     response to an incomming HTTP request.
    /// </summary>
    internal class DefaultResponseHandler : IHttpHandler
    {

        private readonly int _statusCode;
        private readonly string _statusCodeDescription;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.DefaultResponseHandler"/> class.
        /// </summary>
        /// <param name="defaultResponse">The default response.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">defaultResponse</exception>
        public DefaultResponseHandler(ServerDefaultResponse defaultResponse)
        {

            if (Enum.IsDefined(typeof(ServerDefaultResponse), defaultResponse))
            {
                throw new ArgumentOutOfRangeException("defaultResponse");
            }

            _statusCode = (int)defaultResponse;
            _statusCodeDescription = HttpStatusCodes.GetStatusDescription(_statusCode);

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
        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            context.Response.Headers.Clear();
            context.Response.StatusCode = _statusCode;
            context.Response.StatusDescription = _statusCodeDescription;

            if (this.ContextProcessed != null)
            {
                this.ContextProcessed(this, new StumpsContextEventArgs(context));
            }

            return ProcessHandlerResult.Terminate;

        }

    }

}
