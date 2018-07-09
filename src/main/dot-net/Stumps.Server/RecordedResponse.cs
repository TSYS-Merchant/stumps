namespace Stumps.Server
{
    using System;

    /// <summary>
    ///     A class that represents a recorded HTTP response.
    /// </summary>
    public sealed class RecordedResponse : RecordedContextPartBase, IStumpsHttpResponse
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordedResponse" /> class.
        /// </summary>
        /// <param name="response">The <see cref="IStumpsHttpResponse" /> used to initialize the instance.</param>
        /// <param name="decoderHandling">The <see cref="ContentDecoderHandling" /> requirements for the HTTP body.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public RecordedResponse(IStumpsHttpResponse response, ContentDecoderHandling decoderHandling)
            : base(response, decoderHandling)
        {
            response = response ?? throw new ArgumentNullException(nameof(response));

            this.RedirectAddress = response.RedirectAddress;
            this.StatusCode = response.StatusCode;
            this.StatusDescription = response.StatusDescription;
        }

        /// <summary>
        ///     Gets or sets the redirect address.
        /// </summary>
        /// <value>
        ///     The redirect address.
        /// </value>
        public string RedirectAddress
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>
        ///     A value of <c>0</c> or less will not cause a delay.
        /// </remarks>
        public int ResponseDelay
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the HTTP status code for the response.
        /// </summary>
        /// <value>
        ///     The HTTP status code for the response.
        /// </value>
        public int StatusCode
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        public bool TerminateConnection
        {
            get;
            set;
        }

        /// <summary>
        ///     Appends a byte array to the body of the HTTP response.
        /// </summary>
        /// <param name="buffer">The bytes to append to the body of the response.</param>
        public new void AppendToBody(byte[] buffer) => base.AppendToBody(buffer);

        /// <summary>
        ///     Clears the existing body of the HTTP response.
        /// </summary>
        public new void ClearBody() => base.ClearBody();

        /// <summary>
        ///     Examines the body for the classification, and the MD5 hash.
        /// </summary>
        public new void ExamineBody() => base.ExamineBody();
    }
}