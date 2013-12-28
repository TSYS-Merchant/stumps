namespace Stumps {

    using Stumps.Http;

    public class MockHttpContext : IStumpsHttpContext {

        public MockHttpContext() {
            this.Request = new MockHttpRequest();
            this.Response = new MockHttpResponse();
        }

        #region IStumpsHttpContext Members

        public IStumpsHttpRequest Request {
            get;
            set;
        }

        public IStumpsHttpResponse Response {
            get;
            set;
        }

        #endregion

        #region IDisposable Members

        public void Dispose() {

            if ( this.Request != null ) {
                this.Request.Dispose();
                this.Request = null;
            }

            if ( this.Response != null ) {
                this.Response.Dispose();
                this.Response = null;
            }

        }

        #endregion

    }

}
