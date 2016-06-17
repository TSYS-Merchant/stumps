namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     A class representing a recorded HTTP request and response.
    /// </summary>
    public class RecordedContext : IStumpsHttpContext
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RecordedContext" /> class.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext"/> used to initialize the instance.</param>
        /// <param name="decoderHandling">The <see cref="T:Stumps.Server.ContentDecoderHandling" /> requirements for the HTTP body.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public RecordedContext(IStumpsHttpContext context, ContentDecoderHandling decoderHandling)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.Request = new RecordedRequest(context.Request, decoderHandling);
            this.Response = new RecordedResponse(context.Response, decoderHandling);

            this.ReceivedDate = context.ReceivedDate;
            this.UniqueIdentifier = context.UniqueIdentifier;

        }

        /// <summary>
        /// Gets the received date and time the request was received.
        /// </summary>
        /// <value>
        /// The date and time the request was received.
        /// </value>
        public DateTime ReceivedDate
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.Server.RecordedRequest" /> object for the HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.Server.RecordedRequest" /> object for the HTTP request.
        /// </value>
        public RecordedRequest Request
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpRequest" /> object for the HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpRequest" /> object for the HTTP request.
        /// </value>
        IStumpsHttpRequest IStumpsHttpContext.Request
        {
            get { return this.Request; }
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.Server.RecordedResponse" /> object for the HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.Server.RecordedResponse" /> object for the HTTP response.
        /// </value>
        public RecordedResponse Response
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse" /> object for the HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse" /> object for the HTTP response.
        /// </value>
        IStumpsHttpResponse IStumpsHttpContext.Response
        {
            get { return this.Response; }
        }

        /// <summary>
        ///     Gets the unique identifier for the recorded HTTP context.
        /// </summary>
        /// <value>
        ///     The unique identifier for the recorded HTTP context.
        /// </value>
        public Guid UniqueIdentifier
        {
            get;
            private set;
        }

        /// <summary>Gets or sets a value indicating whether [ignore SSL errors].</summary>
        /// <value><c>true</c> if [ignore SSL errors]; otherwise, <c>false</c>.</value>
        public bool IgnoreSslErrors { get; set; }
    }

}