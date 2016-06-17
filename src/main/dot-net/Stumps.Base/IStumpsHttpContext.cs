namespace Stumps
{

    using System;

    /// <summary>
    ///     An interface that represents the complete context for an HTTP request.
    /// </summary>
    public interface IStumpsHttpContext
    {

        /// <summary>
        ///     Gets the received date and time the request was received.
        /// </summary>
        /// <value>
        ///     The date and time the request was received.
        /// </value>
        DateTime ReceivedDate { get; }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpRequest"/> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpRequest"/> object for the current HTTP request.
        /// </value>
        IStumpsHttpRequest Request { get; }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse"/> object for the current HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse"/> object for the current HTTP response.
        /// </value>
        IStumpsHttpResponse Response { get; }

        /// <summary>
        ///     Gets the unique identifier for the HTTP context.
        /// </summary>
        /// <value>
        ///     The unique identifier for the HTTP context.
        /// </value>
        Guid UniqueIdentifier { get; }

        /// <summary>
        /// Gets or sets a value indicating whether [ignore SSL errors].
        /// </summary>
        /// <value><c>true</c> if [ignore SSL errors]; otherwise, <c>false</c>.</value>
        bool IgnoreSslErrors { get; set; }

    }

}