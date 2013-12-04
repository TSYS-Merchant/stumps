namespace Stumps.Http {

    using System;
    using System.IO;
    using System.Net;

    internal sealed class StumpsHttpRequest : IStumpsHttpRequest {

        private readonly HttpListenerRequest _request;
        private MemoryStream _requestStream;
        private bool _disposed = false;

        public StumpsHttpRequest(HttpListenerRequest request) {

            _request = request;

            _requestStream = new MemoryStream();

            int bytesRead;
            byte[] buffer = new byte[4096];

            while ( (bytesRead = _request.InputStream.Read(buffer, 0, 4096)) > 0 ) {
                _requestStream.Write(buffer, 0, bytesRead);
            }

            // Reset the position
            _requestStream.Position = 0;

        }

        public string ContentType {
            get { return _request.ContentType; }
        }

        public System.Collections.Specialized.NameValueCollection Headers {
            get { return _request.Headers; }
        }

        public string HttpMethod {
            get { return _request.HttpMethod; }
        }

        public System.IO.Stream InputStream {
            get { return _requestStream; }
        }

        public bool IsSecureConnection {
            get { return _request.IsSecureConnection; }
        }

        public IPEndPoint LocalEndPoint {
            get { return _request.LocalEndPoint; }
        }

        public Version ProtocolVersion {
            get { return _request.ProtocolVersion; }
        }

        public System.Collections.Specialized.NameValueCollection QueryString {
            get { return _request.QueryString; }
        }

        public string RawUrl {
            get { return _request.RawUrl; }
        }

        public string Referer {
            get {
                string referer = null;

                if ( _request.UrlReferrer != null ) {
                    referer = _request.UrlReferrer.ToString();
                }

                return referer;
            }
        }

        public IPEndPoint RemoteEndPoint {
            get { return _request.RemoteEndPoint; }
        }

        public Uri Url {
            get { return _request.Url; }
        }

        public string UserAgent {
            get { return _request.UserAgent; }
        }

        #region IDisposable Members

        public void Dispose() {

            if ( _requestStream != null && !_disposed ) {
                _requestStream.Dispose();
                _requestStream = null;
            }

            _disposed = true;

            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
