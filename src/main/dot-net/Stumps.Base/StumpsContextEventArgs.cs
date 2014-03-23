namespace Stumps
{

    using System;
    using Stumps.Http;

    /// <summary>
    ///     Provides data for an event that occurred for a <see cref="T:Stumps.IStumpsHttpContext"/>.
    /// </summary>
    public sealed class StumpsContextEventArgs : EventArgs
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsContextEventArgs" /> class.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext" /> associated with the event.</param>
        internal StumpsContextEventArgs(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var stumpsResponse = context.Response as StumpsHttpResponse;
            
            if (stumpsResponse != null)
            {
                this.ResponseOrigin = stumpsResponse.Origin;
                this.StumpId = stumpsResponse.StumpId;
            }
            else
            {
                this.ResponseOrigin = HttpResponseOrigin.Unprocessed;
                this.StumpId = null;
            }

            this.Context = context;

        }

        /// <summary>
        /// Gets the <see cref="T:Stumps.IStumpsHttpContext"/> associated with the event.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.IStumpsHttpContext"/> associated with the event.
        /// </value>
        public IStumpsHttpContext Context
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Gets the origin of the HTTP response.
        /// </summary>
        /// <value>
        ///     The origin of the HTTP response.
        /// </value>
        public HttpResponseOrigin ResponseOrigin
        {
            get; 
            private set;
        }

        /// <summary>
        ///     Gets the unique identifier for the Stump that processed the request.
        /// </summary>
        /// <value>
        ///     The unique identifier for the Stump that processed the request.
        /// </value>
        /// <remarks>If a <see cref="T:Stumps.Stump"/> was not used to process the request, the value will be <c>null</c>.</remarks>
        public string StumpId
        {
            get;
            private set;
        }

    }

}