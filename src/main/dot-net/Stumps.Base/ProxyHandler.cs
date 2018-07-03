namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that proxies requests to an external host.
    /// </summary>
    internal class ProxyHandler : IHttpHandler
    {
        private static readonly List<string> ReservedHeaders = new List<string>()
        {
            "accept",
            "connection",
            "content-length",
            "content-type",
            "expect",
            "date",
            "host",
            "if-modified-since",
            "range",
            "referer",
            "transfer-encoding",
            "user-agent",
        };

        private readonly Uri _externalHostUri;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.ProxyHandler" /> class.
        /// </summary>
        /// <param name="externalHostUri">The external host URI.</param>
        public ProxyHandler(Uri externalHostUri)
        {
            _externalHostUri = externalHostUri ?? throw new ArgumentNullException(nameof(externalHostUri));
        }

        /// <summary>
        ///     Occurs when an incoming HTTP requst is processed and responded to by the HTTP handler.
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
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            var remoteUrl = BuildRemoteUrlFromContext(context);

            // Create a new HTTP web request
            var remoteWebRequest = (HttpWebRequest)WebRequest.Create(remoteUrl);
            remoteWebRequest.AllowAutoRedirect = false;

            // Populate the headers for the new HTTP request from the incoming HTTP request
            PopulateRemoteHeadersFromContext(context, remoteWebRequest);

            // Populate the HTTP body for the request
            var webResponseResult = await PopulateRemoteBodyFromContext(context, remoteWebRequest);
            var continueProcess = webResponseResult.Success;

            // Execute the remote web request
            if (continueProcess)
            {
                await ExecuteRemoteWebRequest(remoteWebRequest, webResponseResult);
            }

            if (!continueProcess)
            {
                // Return a response to the client that the service is unavailable at this time.
                context.Response.StatusCode = HttpStatusCodes.HttpServiceUnavailable;
                context.Response.StatusDescription =
                    HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpServiceUnavailable);
            }
            else if (webResponseResult.Response != null)
            {
                // Write the headers and the body of the response from the remote HTTP request
                // to the incoming HTTP context.
                WriteContextHeadersFromResponse(context, webResponseResult.Response);
                await WriteContextBodyFromRemoteResponse(context, webResponseResult.Response);
                context.Response.StatusCode = (int)webResponseResult.Response.StatusCode;
                context.Response.StatusDescription = webResponseResult.Response.StatusDescription;

                var disposable = webResponseResult.Response as IDisposable;
                disposable.Dispose();
            }

            if (context.Response is StumpsHttpResponse stumpsResponse)
            {
                stumpsResponse.Origin = HttpResponseOrigin.RemoteServer;
            }

            this.ContextProcessed?.Invoke(this, new StumpsContextEventArgs(context));

            return await Task.FromResult<ProcessHandlerResult>(ProcessHandlerResult.Continue);
        }

        /// <summary>
        ///     Builds the remote URL from context.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <returns>
        ///     A string representing the URL for the remote server.
        /// </returns>
        private string BuildRemoteUrlFromContext(IStumpsHttpContext incomingHttpContext)
        {
            var urlPath = incomingHttpContext.Request.RawUrl;
            var urlHost = _externalHostUri.AbsoluteUri;
            urlPath = urlPath.StartsWith("/", StringComparison.Ordinal) ? urlPath.Remove(0, 1) : urlPath;
            var url = urlHost + urlPath;

            return url;
        }

        /// <summary>
        ///     Executes the remote web request.
        /// </summary>
        /// <param name="remoteWebRequest">The remote web request.</param>
        /// <param name="remoteWebResponseResult">The remote web response.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors are logged.")]
        private async Task ExecuteRemoteWebRequest(HttpWebRequest remoteWebRequest, WebResponseResult remoteWebResponseResult)
        {
            remoteWebResponseResult.Success = true;

            try
            {
                var webResponse = await remoteWebRequest.GetResponseAsync();
                remoteWebResponseResult.Response = (HttpWebResponse)webResponse;
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    remoteWebResponseResult.Response = (HttpWebResponse)wex.Response;
                }
                else
                {
                    remoteWebResponseResult.Success = false;
                }
            }
            catch (Exception)
            {
                remoteWebResponseResult.Success = false;
            }
        }

        /// <summary>
        ///     Gets the value of a header.
        /// </summary>
        /// <param name="headers">The dictionary of possible headers.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="defaultValue">The default value to use if the header is not found.</param>
        /// <returns>
        ///     A <see cref="T:System.String"/> representing the value of the header.
        /// </returns>
        private string GetHeaderValue(Dictionary<string, string> headers, string headerName, string defaultValue)
        {
            var headerValue = headers.ContainsKey(headerName) ? headers[headerName] : defaultValue;
            return headerValue;
        }

        /// <summary>
        ///     Populates the remote body from context.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <param name="remoteWebRequest">The remote web request.</param>
        /// <returns>
        ///     Returns a <see cref="T:Stumps.ProxyHandler.WebResponseResult"/> containing the HTTP response and status.
        /// </returns>
        private async Task<WebResponseResult> PopulateRemoteBodyFromContext(
            IStumpsHttpContext incomingHttpContext, 
            HttpWebRequest remoteWebRequest)
        {
            var result = new WebResponseResult
            {
                Success = true
            };

            try
            {
                if (incomingHttpContext.Request.BodyLength > 0)
                {
                    remoteWebRequest.ContentLength = incomingHttpContext.Request.BodyLength;
                    var requestStream = await remoteWebRequest.GetRequestStreamAsync();
                    await requestStream.WriteAsync(incomingHttpContext.Request.GetBody(), 0, incomingHttpContext.Request.BodyLength);
                }
            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    result.Response = (HttpWebResponse)wex.Response;
                }
                else
                {
                    result.Success = false;
                }
            }

            return result;
        }

        /// <summary>
        ///     Populates the remote headers from context.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <param name="remoteWebRequest">The remote web request.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors are logged.")]
        private void PopulateRemoteHeadersFromContext(IStumpsHttpContext incomingHttpContext, HttpWebRequest remoteWebRequest)
        {
            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in incomingHttpContext.Request.Headers.HeaderNames)
            {
                headers.Add(key, incomingHttpContext.Request.Headers[key]);
            }

            remoteWebRequest.Method = incomingHttpContext.Request.HttpMethod;
            remoteWebRequest.Accept = GetHeaderValue(headers, "accept", null);
            remoteWebRequest.ContentType = GetHeaderValue(headers, "content-type", string.Empty);
            remoteWebRequest.Referer = GetHeaderValue(headers, "referer", null);
            remoteWebRequest.TransferEncoding = GetHeaderValue(headers, "transfer-encoding", null);
            remoteWebRequest.UserAgent = GetHeaderValue(headers, "user-agent", null);

            ReservedHeaders.ForEach(header => headers.Remove(header));

            if (headers.Count <= 0)
            {
                return;
            }

            foreach (var key in headers.Keys)
            {
                try
                {
                    remoteWebRequest.Headers.Add(key, headers[key]);
                }
                catch (ArgumentException)
                {
                    // The header could fail to add because it is being referenced
                    // as a property - this is OK.
                    // TODO: Log error
                }
            }
        }

        /// <summary>
        ///     Writes the context body from the remote response.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        private async Task WriteContextBodyFromRemoteResponse(IStumpsHttpContext incomingHttpContext, HttpWebResponse remoteWebResponse)
        {
            if (remoteWebResponse.ContentLength != 0)
            {
                var responseStream = remoteWebResponse.GetResponseStream();
                incomingHttpContext.Response.ClearBody();
                incomingHttpContext.Response.AppendToBody(await StreamUtility.ConvertStreamToByteArray(responseStream));
            }
        }

        /// <summary>
        ///     Writes the context headers from the remote response.
        /// </summary>
        /// <param name="incomingHttpContext">The incoming HTTP context.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        private void WriteContextHeadersFromResponse(
            IStumpsHttpContext incomingHttpContext, HttpWebResponse remoteWebResponse)
        {
            incomingHttpContext.Response.Headers.Clear();

            foreach (var headerName in remoteWebResponse.Headers.AllKeys)
            {
                incomingHttpContext.Response.Headers[headerName] = remoteWebResponse.Headers[headerName];
            }
        }

        /// <summary>
        ///     A class which contains the HTTP response executed within an asynchronous context.
        /// </summary>
        private class WebResponseResult
        {
            /// <summary>
            /// Gets or sets the HTTP web response.
            /// </summary>
            /// <value>
            /// The HTTP web response.
            /// </value>
            public HttpWebResponse Response
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the HTTP request was successful.
            /// </summary>
            /// <value>
            ///   <c>true</c> if successful; otherwise, <c>false</c>.
            /// </value>
            public bool Success
            {
                get;
                set;
            }
        }
    }
}