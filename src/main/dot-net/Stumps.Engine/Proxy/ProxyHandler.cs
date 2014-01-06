namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using System.Net;
    using Stumps.Http;
    using Stumps.Logging;
    using Stumps.Utility;

    internal class ProxyHandler : IHttpHandler {

        private readonly ProxyEnvironment _environment;
        private readonly Uri _externalHostUri;
        private readonly ILogger _logger;

        public ProxyHandler(ProxyEnvironment environment, ILogger logger) {

            if ( environment == null ) {
                throw new ArgumentNullException("environment");
            }

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            _environment = environment;
            _logger = logger;

            _externalHostUri = createUriFromEnvironment();

        }

        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context) {

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            processRequest(context);

            return ProcessHandlerResult.Continue;

        }

        private string buildRemoteUrlFromContext(IStumpsHttpContext incommingHttpContext) {

            var urlPath = incommingHttpContext.Request.RawUrl;

            var urlHost = _externalHostUri.AbsoluteUri;

            urlPath = urlPath.StartsWith("/", StringComparison.Ordinal) ? urlPath : urlPath + "/";

            var url = urlHost + urlPath;

            return url;

        }

        private Uri createUriFromEnvironment() {

            var schema = _environment.UseSsl ? "https://" : "http://";

            var externalHostUri = new Uri(schema + _environment.ExternalHostName + "/");

            return externalHostUri;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors are logged.")]
        private bool executeRemoteWebRequest(HttpWebRequest remoteWebRequest, ref HttpWebResponse remoteWebResponse) {

            var success = true;

            try {
                remoteWebResponse = (HttpWebResponse) remoteWebRequest.GetResponse();
            }
            catch ( WebException wex ) {

                if ( wex.Response != null ) {
                    remoteWebResponse = (HttpWebResponse) wex.Response;
                }
                else {
                    _logger.LogException(Resources.LogProxyRemoteError, wex);
                    success = false;
                }

            }
            catch ( Exception ex ) {
                _logger.LogException(Resources.LogProxyRemoteError, ex);
                success = false;
            }

            return success;

        }

        private string getHeaderValue(Dictionary<string, string> headers, string headerName, string defaultValue) {

            var headerValue = headers.ContainsKey(headerName) ? headers[headerName] : defaultValue;
            return headerValue;

        }

        private bool populateRemoteBodyFromContext(IStumpsHttpContext incommingHttpContext, HttpWebRequest remoteWebRequest, ref HttpWebResponse remoteWebResponse) {

            var success = true;

            try {

                incommingHttpContext.Request.InputStream.Position = 0;

                var body = StreamUtility.ConvertStreamToByteArray(incommingHttpContext.Request.InputStream);

                incommingHttpContext.Request.InputStream.Position = 0;

                if ( body != null && body.Length > 0 ) {
                    remoteWebRequest.ContentLength = body.Length;
                    var requestStream = remoteWebRequest.GetRequestStream();
                    requestStream.Write(body, 0, body.Length);
                }

            }
            catch ( WebException wex ) {
                if ( wex.Response != null ) {
                    remoteWebResponse = (HttpWebResponse) wex.Response;
                }
                else {
                    success = false;
                }
            }

            return success;

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors are logged.")]
        private void populateRemoteHeadersFromContext(IStumpsHttpContext incommingHttpContext, HttpWebRequest remoteWebRequest) {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach ( var key in incommingHttpContext.Request.Headers.AllKeys ) {
                headers.Add(key, incommingHttpContext.Request.Headers[key]);
            }

            remoteWebRequest.Method = incommingHttpContext.Request.HttpMethod;
            remoteWebRequest.Accept = getHeaderValue(headers, "accept", null);
            remoteWebRequest.ContentType = getHeaderValue(headers, "content-type", incommingHttpContext.Request.ContentType);
            remoteWebRequest.Referer = getHeaderValue(headers, "referer", incommingHttpContext.Request.Referer);
            remoteWebRequest.TransferEncoding = getHeaderValue(headers, "transfer-encoding", null);
            remoteWebRequest.UserAgent = getHeaderValue(headers, "user-agent", incommingHttpContext.Request.UserAgent);

            headers.Remove("accept");
            headers.Remove("connection");
            headers.Remove("content-length");
            headers.Remove("content-type");
            headers.Remove("expect");
            headers.Remove("date");
            headers.Remove("host");
            headers.Remove("if-modified-since");
            headers.Remove("range");
            headers.Remove("referer");
            headers.Remove("transfer-encoding");
            headers.Remove("user-agent");

            if ( headers.Count > 0 ) {

                foreach ( var key in headers.Keys ) {
                    try {
                        remoteWebRequest.Headers.Add(key, headers[key]);
                    }
                    catch ( Exception ex ) {
                        // The header could fail to add because it is being referenced
                        // as a property - this is OK.
                        _logger.LogException(Resources.LogProxyHeaderError, ex);
                    }
                }
            }

        }

        private void processRequest(IStumpsHttpContext incommingHttpContext) {

            var remoteUrl = buildRemoteUrlFromContext(incommingHttpContext);

            // Create a new HTTP web request
            var remoteWebRequest = (HttpWebRequest) WebRequest.Create(remoteUrl);
            remoteWebRequest.AllowAutoRedirect = false;

            // Populate the headers for the new HTTP request from the incoming HTTP request
            populateRemoteHeadersFromContext(incommingHttpContext, remoteWebRequest);

            // Setup a repsponse used by the class
            HttpWebResponse response = null;

            // Populate the HTTP body for the request
            var continueProcess = populateRemoteBodyFromContext(incommingHttpContext, remoteWebRequest, ref response);

            // Execute the remote web request
            if ( continueProcess ) {
                continueProcess = executeRemoteWebRequest(remoteWebRequest, ref response);
            }

            if ( !continueProcess ) {
                // Return a response to the client that the service is unavailable at this time.
                incommingHttpContext.Response.StatusCode = HttpStatusCodes.HttpServiceUnavailable;
                incommingHttpContext.Response.StatusDescription = HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpServiceUnavailable);
            }
            else if ( response != null ) {
                // Write the headers and the body of the response from the remote HTTP request
                // to the incoming HTTP context.
                writeContextHeadersFromResponse(incommingHttpContext, response);
                writeContextBodyFromRemoteResponse(incommingHttpContext, response);
                incommingHttpContext.Response.StatusCode = (int) response.StatusCode;
                incommingHttpContext.Response.StatusDescription = response.StatusDescription;

                var disposable = response as IDisposable;
                disposable.Dispose();

            }

        }

        private void writeContextBodyFromRemoteResponse(IStumpsHttpContext incommingHttpContext, HttpWebResponse remoteWebResponse) {

            if ( remoteWebResponse.ContentLength != 0 ) {
                var responseStream = remoteWebResponse.GetResponseStream();

                StreamUtility.CopyStream(responseStream, incommingHttpContext.Response.OutputStream);

            }

        }

        private void writeContextHeadersFromResponse(IStumpsHttpContext incommingHttpContext, HttpWebResponse remoteWebResponse) {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach ( var key in remoteWebResponse.Headers.AllKeys ) {
                headers.Add(key, remoteWebResponse.Headers[key]);
            }

            incommingHttpContext.Response.Headers.Clear();
            incommingHttpContext.Response.ContentType = remoteWebResponse.ContentType;

            if ( headers.ContainsKey("transfer-encoding") && headers["transfer-encoding"].Equals("chunked", StringComparison.OrdinalIgnoreCase) ) {
                incommingHttpContext.Response.SendChunked = true;
            }

            headers.Remove("content-length");
            headers.Remove("content-type");

            // The following headers should not be necessary - re-enable them if we see
            // a need in the future.

            //headers.Remove("accept");
            //headers.Remove("connection");
            //headers.Remove("expect");
            //headers.Remove("date");
            //headers.Remove("host");
            //headers.Remove("if-modified-since");
            //headers.Remove("range");
            //headers.Remove("referer");
            headers.Remove("transfer-encoding");
            headers.Remove("keep-alive");
            //headers.Remove("user-agent");

            foreach ( var key in headers.Keys ) {
                incommingHttpContext.Response.Headers.Add(key, headers[key]);
            }

        }

    }

}
