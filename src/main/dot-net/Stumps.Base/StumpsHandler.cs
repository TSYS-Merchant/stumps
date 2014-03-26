namespace Stumps
{

    using System;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that processes HTTP requests
    ///     against a list of Stumps.
    /// </summary>
    internal class StumpsHandler : IHttpHandler
    {

        private readonly IStumpsManager _stumpsManager;
        private volatile bool _handlerEnabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsHandler" /> class.
        /// </summary>
        /// <param name="stumpsManager">The stumps manager.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsManager"/> is <c>null</c>.</exception>
        public StumpsHandler(IStumpsManager stumpsManager)
        {

            if (stumpsManager == null)
            {
                throw new ArgumentNullException("stumpsManager");
            }

            _stumpsManager = stumpsManager;
            _handlerEnabled = true;

        }

        /// <summary>
        ///     Occurs when an incomming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        /// Gets or sets a value indicating whether [enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enabled]; otherwise, <c>false</c>.
        /// </value>
        public bool Enabled
        {
            get { return _handlerEnabled; }
            set { _handlerEnabled = value; }
        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        ///     A member of the <see cref="T:Stumps.Http.ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Early exit, if all Stumps are disabled
            if (!_handlerEnabled)
            {
                return ProcessHandlerResult.Continue;
            }

            var result = ProcessHandlerResult.Continue;

            var stump = _stumpsManager.FindStumpForContext(context);

            if (stump != null)
            {

                PopulateResponse(context, stump);

                var stumpsResponse = context.Response as StumpsHttpResponse;

                if (stumpsResponse != null)
                {
                    stumpsResponse.Origin = HttpResponseOrigin.Stump;
                    stumpsResponse.StumpId = stump.StumpId;
                }

                if (this.ContextProcessed != null)
                {
                    this.ContextProcessed(this, new StumpsContextEventArgs(context));
                }
                
                result = ProcessHandlerResult.Terminate;

            }

            return result;

        }

        /// <summary>
        ///     Populates the response of the HTTP context from the Stump.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> used to populate the response.</param>
        private void PopulateResponse(IStumpsHttpContext incommingHttpContext, Stump stump)
        {

            // Write the status code information
            incommingHttpContext.Response.StatusCode = stump.Response.StatusCode;
            incommingHttpContext.Response.StatusDescription = stump.Response.StatusDescription;

            // Write the headers
            incommingHttpContext.Response.Headers.Clear();

            stump.Response.Headers.CopyTo(incommingHttpContext.Response.Headers);

            // Write the body
            incommingHttpContext.Response.ClearBody();
            
            if (stump.Response.BodyLength > 0)
            {
                var buffer = stump.Response.GetBody();
                if (stump.Response.Headers["Content-Encoding"] != null)
                {
                    var encoder = new ContentEncoder(stump.Response.Headers["Content-Encoding"]);
                    buffer = encoder.Encode(buffer);
                }

                incommingHttpContext.Response.AppendToBody(buffer);
            }

        }

    }

}