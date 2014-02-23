namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using Stumps.Logging;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that processes HTTP requests
    ///     against a list of Stumps.
    /// </summary>
    internal class StumpsHandler : IHttpHandler
    {

        private readonly ProxyEnvironment _environment;
        private readonly ILogger _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.StumpsHandler"/> class.
        /// </summary>
        /// <param name="environment">The environment for the proxy server.</param>
        /// <param name="logger">The logger used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="environment"/> is <c>null</c>.
        /// or
        /// <paramref name="logger"/> is <c>null</c>.
        /// </exception>
        public StumpsHandler(ProxyEnvironment environment, ILogger logger)
        {

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _environment = environment;
            _logger = logger;

        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Http.IStumpsHttpContext" /> representing both the incoming request and the response.</param>
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

            if (_environment.RecordTraffic)
            {
                return ProcessHandlerResult.Continue;
            }

            var result = ProcessHandlerResult.Continue;
            var stump = _environment.Stumps.FindStump(context);

            if (stump != null)
            {

                PopulateResponse(context, stump.Contract.Response);
                _environment.IncrementStumpsServed();

                result = ProcessHandlerResult.Terminate;

            }

            return result;

        }

        /// <summary>
        ///     Populates the response of the HTTP context from the Stump.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="recordedResponse">The recorded response.</param>
        private void PopulateResponse(IStumpsHttpContext context, RecordedResponse recordedResponse)
        {

            context.Response.StatusCode = recordedResponse.StatusCode;
            context.Response.StatusDescription = recordedResponse.StatusDescription;

            WriteHeaders(context, recordedResponse);
            WriteContextBody(context, recordedResponse);

        }

        /// <summary>
        ///     Writes the body to the context response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="recordedResponse">The recorded response.</param>
        private void WriteContextBody(
            IStumpsHttpContext incommingHttpContext, RecordedResponse recordedResponse)
        {

            var buffer = recordedResponse.Body;
            var header = recordedResponse.FindHeader("Content-Encoding");

            if (header != null)
            {
                var encoder = new ContentEncoding(header.Value);
                buffer = encoder.Encode(buffer);
            }

            incommingHttpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);

        }

        /// <summary>
        /// Writes the headers to the context response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="recordedResponse">The recorded response.</param>
        private void WriteHeaders(
            IStumpsHttpContext incommingHttpContext, RecordedResponse recordedResponse)
        {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var header in recordedResponse.Headers)
            {
                headers.Add(header.Name, header.Value);
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

            // The following headers should not be necessary - re-enable them if we see a need in the future.

            ////headers.Remove("accept");
            ////headers.Remove("connection");
            ////headers.Remove("expect");
            ////headers.Remove("date");
            ////headers.Remove("host");
            ////headers.Remove("if-modified-since");
            ////headers.Remove("range");
            ////headers.Remove("referer");
            headers.Remove("transfer-encoding");
            headers.Remove("keep-alive");
            ////headers.Remove("user-agent");

            foreach (var key in headers.Keys)
            {
                incommingHttpContext.Response.Headers.Add(key, headers[key]);
            }

        }

    }

}