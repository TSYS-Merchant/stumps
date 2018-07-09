namespace Stumps
{
    using System;
    using Stumps.Http;

    /// <summary>
    ///     Provides data for an event that occurred for a <see cref="IStumpsHttpContext"/>.
    /// </summary>
    public sealed class StumpsContextEventArgs : EventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsContextEventArgs" /> class.
        /// </summary>
        /// <param name="context">The <see cref="IStumpsHttpContext" /> associated with the event.</param>
        internal StumpsContextEventArgs(IStumpsHttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Response is StumpsHttpResponse stumpsResponse)
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
        /// Gets the <see cref="IStumpsHttpContext"/> associated with the event.
        /// </summary>
        /// <value>
        /// The <see cref="IStumpsHttpContext"/> associated with the event.
        /// </value>
        public IStumpsHttpContext Context
        {
            get; 
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
        }

        /// <summary>
        ///     Gets the unique identifier for the Stump that processed the request.
        /// </summary>
        /// <value>
        ///     The unique identifier for the Stump that processed the request.
        /// </value>
        /// <remarks>If a <see cref="Stump"/> was not used to process the request, the value will be <c>null</c>.</remarks>
        public string StumpId
        {
            get;
        }
    }
}