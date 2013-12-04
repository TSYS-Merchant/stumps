namespace Stumps.Http {

    using System;
    using System.Net;
    using System.IO;
    using Stumps.Utility;

    internal sealed class StumpsHttpResponse : IStumpsHttpResponse {

        private readonly HttpListenerResponse _response;
        private MemoryStream _responseStream;
        private bool _sendChunked;
        private string _statusDescription;
        private int _statusCode;
        private bool _disposed;

        public StumpsHttpResponse(HttpListenerResponse response) {
            _response = response;
            _responseStream = new MemoryStream();

            _statusCode = HttpStatusCodes.HttpOk;
            _statusDescription = HttpStatusCodes.GetStatusDescription(_statusCode);
        }

        public string ContentType {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public HttpListenerResponse ListenerResponse {
            get { return _response; }
        }

        public WebHeaderCollection Headers {
            get { return _response.Headers; }
        }

        public Stream OutputStream {
            get { return _responseStream; }
        }

        public bool SendChunked {
            get { return _sendChunked; }
            set { _sendChunked = value; }
        }

        public int StatusCode {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        public string StatusDescription {
            get { return _statusDescription; }
            set { _statusDescription = value; }
        }

        public void AddHeader(string name, string value) {
            _response.AddHeader(name, value);
        }

        public void AppendHeader(string name, string value) {
            _response.AppendHeader(name, value);
        }

        public void ClearOutputStream() {
            _responseStream.Close();
            _responseStream = new MemoryStream();
        }

        public void FlushResponse() {

            // Dump out the response output stream
            StreamUtility.CopyStream(_responseStream, _response.OutputStream);

            // Complete and close the request
            _responseStream.Close();
            _response.OutputStream.Close();
            _response.Close();

        }

        public void Redirect(string url) {
            _response.Redirect(url);
        }

        #region IDisposable Members

        public void Dispose() {

            if ( _responseStream != null && !_disposed ) {
                _responseStream.Dispose();
                _responseStream = null;
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion

    }

}
