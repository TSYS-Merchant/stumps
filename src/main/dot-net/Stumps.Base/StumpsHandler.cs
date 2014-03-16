namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using Stumps.Utility;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that processes HTTP requests
    ///     against a list of Stumps.
    /// </summary>
    internal class StumpsHandler : IHttpHandler
    {

        private readonly IStumpsManager _stumpsManager;

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
        ///     A member of the <see cref="T:Stumps.Http.ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // TODO: Continue if we are recording traffic
            var result = ProcessHandlerResult.Continue;

            var stump = _stumpsManager.FindStumpForContext(context);

            if (stump != null)
            {

                PopulateResponse(context, stump);

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
        /// <param name="context">The HTTP context.</param>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> used to populate the response.</param>
        private void PopulateResponse(IStumpsHttpContext context, Stump stump)
        {

            context.Response.StatusCode = stump.Response.StatusCode;
            context.Response.StatusDescription = stump.Response.StatusDescription;

            WriteHeaders(context, stump.Response);
            WriteContextBody(context, stump.Response);

        }

        /// <summary>
        ///     Writes the body to the context response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="stumpResponse">The recorded to send back.</param>
        private void WriteContextBody(
            IStumpsHttpContext incommingHttpContext, IStumpsHttpResponse stumpResponse)
        {

            var buffer = StreamUtility.ConvertStreamToByteArray(stumpResponse.OutputStream);
            var encodingMethod = stumpResponse.Headers["Content-Encoding"];

            if (encodingMethod != null)
            {
                var encoder = new ContentEncoder(encodingMethod);
                buffer = encoder.Encode(buffer);
            }

            incommingHttpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);

        }

        /// <summary>
        /// Writes the headers to the context response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="stumpResponse">The recorded response.</param>
        private void WriteHeaders(
            IStumpsHttpContext incommingHttpContext, IStumpsHttpResponse stumpResponse)
        {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var headerName in stumpResponse.Headers.HeaderNames)
            {
                headers.Add(headerName, stumpResponse.Headers[headerName]);
            }

            incommingHttpContext.Response.Headers.Clear();

            if (headers.ContainsKey("content-type"))
            {
                incommingHttpContext.Response.ContentType = headers["content-type"];
            }

            if (headers.ContainsKey("transfer-encoding") &&
                headers["transfer-encoding"].Equals("chunked", StringComparison.OrdinalIgnoreCase))
            {
                incommingHttpContext.Response.SendChunked = true;
            }

            headers.Remove("content-length");
            headers.Remove("content-type");
            headers.Remove("transfer-encoding");
            headers.Remove("keep-alive");

            foreach (var key in headers.Keys)
            {
                incommingHttpContext.Response.Headers.Add(key, headers[key]);
            }

        }

    }

}