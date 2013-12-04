namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using Stumps.Logging;
    using Stumps.Utility;

    internal class StumpsHandler : IHttpHandler {

        private readonly ProxyEnvironment _environment;
        private readonly ILogger _logger;

        public StumpsHandler(ProxyEnvironment environment, ILogger logger) {

            if ( environment == null ) {
                throw new ArgumentNullException("environment");
            }

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            _environment = environment;
            _logger = logger;

        }

        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context) {

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            if ( _environment.RecordTraffic ) {
                return ProcessHandlerResult.Continue;
            }

            var result = processRequest(context);

            return result;

        }

        private void populateResponse(IStumpsHttpContext context, RecordedResponse recordedResponse) {

            context.Response.StatusCode = recordedResponse.StatusCode;
            context.Response.StatusDescription = recordedResponse.StatusDescription;

            var headerDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach ( var header in recordedResponse.Headers ) {
                headerDictionary.Add(header.Name, header.Value);
            }

            writeHeadersFromResponse(context, headerDictionary, recordedResponse);
            writeContextBodyFromResponse(context, headerDictionary, recordedResponse);

        }

        private ProcessHandlerResult processRequest(IStumpsHttpContext context) {

            var result = ProcessHandlerResult.Continue;
            var stump = _environment.Stumps.FindStump(context);

            if ( stump != null ) {

                populateResponse(context, stump.Contract.Response);
                _environment.IncrementStumpsServed();

                result = ProcessHandlerResult.Terminate;

            }

            return result;

        }

        private void writeContextBodyFromResponse(IStumpsHttpContext incommingHttpContext, Dictionary<string, string> headers, RecordedResponse recordedResponse) {

            var header = recordedResponse.FindHeader("Content-Encoding");

            var buffer = recordedResponse.Body;

            if ( header != null && header.Value.Equals("gzip", StringComparison.OrdinalIgnoreCase) ) {
                buffer = CompressionUtility.CompressGzipByteArray(buffer);
            }
            else if ( header != null && header.Value.Equals("deflate", StringComparison.Ordinal) ) {
                buffer = CompressionUtility.CompressDeflateByteArray(buffer);
            }

            incommingHttpContext.Response.OutputStream.Write(buffer, 0, buffer.Length);

        }

        private void writeHeadersFromResponse(IStumpsHttpContext incommingHttpContext, Dictionary<string, string> headers, RecordedResponse recordedResponse) {

            incommingHttpContext.Response.Headers.Clear();

            if ( headers.ContainsKey("content-type") ) {
                incommingHttpContext.Response.ContentType = headers["content-type"];
            }

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
