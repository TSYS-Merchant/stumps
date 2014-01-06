namespace Stumps {

    using System;
    using Stumps.Http;

    public class MockHttpRequest : IStumpsHttpRequest {

        #region IStumpsHttpRequest Members

        public string ContentType {
            get;
            set;
        }

        public System.Collections.Specialized.NameValueCollection Headers {
            get;
            set;
        }

        public string HttpMethod {
            get;
            set;
        }

        public System.IO.Stream InputStream {
            get;
            set;
        }

        public bool IsSecureConnection {
            get;
            set;
        }

        public System.Net.IPEndPoint LocalEndPoint {
            get;
            set;
        }

        public Version ProtocolVersion {
            get;
            set;
        }

        public System.Collections.Specialized.NameValueCollection QueryString {
            get;
            set;
        }

        public string RawUrl {
            get;
            set;
        }

        public string Referer {
            get;
            set;
        }

        public System.Net.IPEndPoint RemoteEndPoint {
            get;
            set;
        }

        public Uri Url {
            get;
            set;
        }

        public string UserAgent {
            get;
            set;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {

            if ( this.InputStream != null ) {
                this.InputStream.Close();
                this.InputStream = null;
            }

        }

        #endregion

    }

}
