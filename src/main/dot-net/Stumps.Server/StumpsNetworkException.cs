namespace Stumps.Server
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The exception thrown when a network related error occurs within the Stumps framework.
    /// </summary>
    [Serializable]
    public class StumpsNetworkException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsNetworkException"/> class.
        /// </summary>
        public StumpsNetworkException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsNetworkException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public StumpsNetworkException(string message) : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsNetworkException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public StumpsNetworkException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsNetworkException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected StumpsNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}