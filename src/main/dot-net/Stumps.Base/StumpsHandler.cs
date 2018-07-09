namespace Stumps
{
    using System;
    using System.Threading.Tasks;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="IHttpHandler"/> interface that processes HTTP requests
    ///     against a list of Stumps.
    /// </summary>
    internal class StumpsHandler : IHttpHandler
    {
        /// <summary>
        ///     The minimum amount of time allowed for a delayed response.
        /// </summary>
        private const int MinimumResponseDelay = 0;

        /// <summary>
        ///     The maximum amount of time allowed for a delayed response.
        /// </summary>
        private const int MaximumResponseDelay = 120000;

        private readonly IStumpsManager _stumpsManager;
        private volatile bool _handlerEnabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsHandler" /> class.
        /// </summary>
        /// <param name="stumpsManager">The stumps manager.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsManager"/> is <c>null</c>.</exception>
        public StumpsHandler(IStumpsManager stumpsManager)
        {
            _stumpsManager = stumpsManager ?? throw new ArgumentNullException(nameof(stumpsManager));
            _handlerEnabled = true;
        }

        /// <summary>
        ///     Occurs when an incoming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        ///     Gets or sets a value indicating whether this isntance is enabled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get => _handlerEnabled;
            set => _handlerEnabled = value;
        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        ///     A member of the <see cref="ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            // Early exit, if all Stumps are disabled
            if (!_handlerEnabled)
            {
                return ProcessHandlerResult.Continue;
            }

            var stump = _stumpsManager.FindStumpForContext(context);

            // Early exit if the stump cannot be found
            if (stump == null)
            {
                return ProcessHandlerResult.Continue;
            }

            // Create the response
            var stumpResponse = stump.ResponseFactory.CreateResponse(context.Request);

            if (stumpResponse.ResponseDelay > StumpsHandler.MinimumResponseDelay)
            {
                var delay = stumpResponse.ResponseDelay;
                delay = delay < StumpsHandler.MaximumResponseDelay ? delay : StumpsHandler.MaximumResponseDelay;

                await Task.Delay(delay);
            }

            if (stumpResponse.TerminateConnection)
            {
                return ProcessHandlerResult.DropConnection;
            }

            PopulateResponse(context, stumpResponse);

            if (context.Response is StumpsHttpResponse stumpsResponse)
            {
                stumpsResponse.Origin = HttpResponseOrigin.Stump;
                stumpsResponse.StumpId = stump.StumpId;
            }

            this.ContextProcessed?.Invoke(this, new StumpsContextEventArgs(context));

            return ProcessHandlerResult.Terminate;
        }

        /// <summary>
        ///     Populates the response of the HTTP context from the Stump.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <param name="stumpResponse">The <see cref="IStumpsHttpResponse"/> used to populate the <paramref name="incomingHttpContext"/> response.</param>
        private void PopulateResponse(IStumpsHttpContext incomingHttpContext, IStumpsHttpResponse stumpResponse)
        {
            // Write the status code information
            incomingHttpContext.Response.StatusCode = stumpResponse.StatusCode;
            incomingHttpContext.Response.StatusDescription = stumpResponse.StatusDescription;

            // Write the headers
            incomingHttpContext.Response.Headers.Clear();

            stumpResponse.Headers.CopyTo(incomingHttpContext.Response.Headers);

            // Write the body
            incomingHttpContext.Response.ClearBody();
            
            if (stumpResponse.BodyLength > 0)
            {
                var buffer = stumpResponse.GetBody();
                if (stumpResponse.Headers["Content-Encoding"] != null)
                {
                    var encoder = new ContentEncoder(stumpResponse.Headers["Content-Encoding"]);
                    buffer = encoder.Encode(buffer);
                }

                incomingHttpContext.Response.AppendToBody(buffer);
            }
        }
    }
}