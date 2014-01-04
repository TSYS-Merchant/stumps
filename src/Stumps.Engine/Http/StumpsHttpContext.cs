namespace Stumps.Http {

    using System;
    using System.Net;

    internal sealed class StumpsHttpContext : IStumpsHttpContext {

        private StumpsHttpRequest _request;
        private StumpsHttpResponse _response;

        private bool _disposed;

        public StumpsHttpContext(HttpListenerContext context) {

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            this.ContextId = Guid.NewGuid();

            applyNewContext(context);

        }

        public Guid ContextId { get; private set; }

        public IStumpsHttpRequest Request {
            get { return _request; }
        }

        public IStumpsHttpResponse Response {
            get { return _response; }
        }

        public void End() {

            _response.FlushResponse();

        }

        private void applyNewContext(HttpListenerContext context) {

            _request = new StumpsHttpRequest(context.Request);
            _response = new StumpsHttpResponse(context.Response);

        }

        #region IDisposable Members

        public void Dispose() {


            if ( !_disposed ) {
                if ( _request != null ) {
                    _request.Dispose();
                    _request = null;
                }

                if ( _response != null ) {
                    _response.Dispose();
                    _response = null;
                }
            }

            _disposed = true;

            GC.SuppressFinalize(this);

        }

        #endregion

    }

}
