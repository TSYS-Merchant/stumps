namespace Stumps.Http
{

    using System;
    using System.Net;

    /// <summary>
    ///     A class that represents the complete context for an HTTP request.
    /// </summary>
    internal sealed class StumpsHttpContext : IStumpsHttpContext
    {

        private bool _disposed;
        private StumpsHttpRequest _request;
        private StumpsHttpResponse _response;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Http.StumpsHttpContext"/> class.
        /// </summary>
        /// <param name="context">The <see cref="T:System.Net.HttpListenerContext"/> used to initialize the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public StumpsHttpContext(HttpListenerContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.ContextId = Guid.NewGuid();

            InitializeWithHttpListerContext(context);

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:Stumps.Http.StumpsHttpContext"/> class.
        /// </summary>
        ~StumpsHttpContext()
        {
            Dispose();
        }

        /// <summary>
        ///     Gets the context unique identifier for the request.
        /// </summary>
        /// <value>
        ///     The context unique identifier for the request.
        /// </value>
        public Guid ContextId { get; private set; }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpRequest" /> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpRequest" /> object for the current HTTP request.
        /// </value>
        public IStumpsHttpRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse" /> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse" /> object for the current HTTP request.
        /// </value>
        public IStumpsHttpResponse Response
        {
            get { return _response; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (!_disposed)
            {
                if (_request != null)
                {
                    _request.Dispose();
                    _request = null;
                }

                if (_response != null)
                {
                    _response.Dispose();
                    _response = null;
                }
            }

            _disposed = true;

            GC.SuppressFinalize(this);

        }

        /// <summary>
        ///     Ends the HTTP context and responds to the calling client.
        /// </summary>
        public void End()
        {

            _response.FlushResponse();

        }

        /// <summary>
        ///     Initializes the instance using an <see cref="T:System.Net.HttpListenerContext"/> object.
        /// </summary>
        /// <param name="context">The <see cref="T:System.Net.HttpListenerContext"/> used to initialize the instance.</param>
        private void InitializeWithHttpListerContext(HttpListenerContext context)
        {

            _request = new StumpsHttpRequest(context.Request);
            _response = new StumpsHttpResponse(context.Response);

        }

    }

}