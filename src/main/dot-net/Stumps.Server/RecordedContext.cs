namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     A class representing a recorded HTTP request and response.
    /// </summary>
    public class RecordedContext
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RecordedContext"/> class.
        /// </summary>
        public RecordedContext()
        {
            this.RequestDate = DateTime.Now;
            this.Request = new RecordedRequest();
            this.Response = new RecordedResponse();
        }

        /// <summary>
        ///     Gets the <see cref="T:System.DateTime"/> the HTTP request was received.
        /// </summary>
        /// <value>
        ///     The <see cref="T:System.DateTime"/> the HTTP request was received.
        /// </value>
        public DateTime RequestDate { get; private set; }

        /// <summary>
        ///     Gets or sets the recorded HTTP request.
        /// </summary>
        /// <value>
        ///     The recorded HTTP request.
        /// </value>
        public RecordedRequest Request { get; private set; }

        /// <summary>
        ///     Gets or sets the recorded HTTP response.
        /// </summary>
        /// <value>
        ///     The recorded HTTP response.
        /// </value>
        public RecordedResponse Response { get; private set; }

    }

}