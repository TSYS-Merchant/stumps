namespace Stumps
{
    using System;

    /// <summary>
    ///     An interface that represents a factory capable of creating or returning an HTTP response given an HTTP request.
    /// </summary>
    public interface IStumpResponseFactory : IDisposable
    {
        /// <summary>
        ///     Gets the <see cref="IStumpsHttpResponse"/> at the specified index.
        /// </summary>
        /// <value>
        ///     The <see cref="IStumpsHttpResponse"/> at the specified index.
        /// </value>
        /// <param name="index">The zero-based index of the <see cref="IStumpsHttpResponse"/> to get.</param>
        /// <returns>The <see cref="IStumpsHttpResponse"/> at the specified index.</returns>
        IStumpsHttpResponse this[int index] { get; }

        /// <summary>
        ///     Gets the The behavior of the <see cref="CreateResponse(IStumpsHttpRequest)"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.
        /// </summary>
        /// <value>
        ///     The behavior of the <see cref="CreateResponse(IStumpsHttpRequest)"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.
        /// </value>
        ResponseFactoryBehavior Behavior { get; }

        /// <summary>
        ///     Gets the number of responses contained within the <see cref="IStumpResponseFactory"/>.
        /// </summary>
        /// <value>
        ///     The number of responses contained within the <see cref="IStumpResponseFactory"/>.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Gets the default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance does not contain any responses.
        /// </summary>
        /// <value>
        /// The default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance does not contain any responses.
        /// </value>
        IStumpsHttpResponse FailureResponse { get; set; }

        /// <summary>
        ///     Gets a value indicating whether this instance has a valid response available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has a valid response available; otherwise, <c>false</c>.
        /// </value>
        bool HasResponse { get; }

        /// <summary>
        ///     Adds the specified response to the <see cref="IStumpResponseFactory" />.
        /// </summary>
        /// <param name="response">The <see cref="IStumpsHttpResponse" /> to add to the <see cref="IStumpResponseFactory" />.</param>
        /// <returns>The <see cref="IStumpsHttpResponse"/> added to the object.</returns>
        IStumpsHttpResponse Add(IStumpsHttpResponse response);

        /// <summary>
        ///     Removes all items from the <see cref="IStumpResponseFactory"/>.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Creates an <see cref="IStumpsHttpResponse"/> object based on an incoming <see cref="IStumpsHttpRequest"/>
        ///     which is returned for a Stump positivly matching all necessary criteria.
        /// </summary>
        /// <param name="request">The incoming <see cref="IStumpsHttpRequest"/>.</param>
        /// <returns>
        ///     An <see cref="IStumpsHttpResponse"/> object based on an incoming <see cref="IStumpsHttpRequest"/>.
        /// </returns>
        IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request);

        /// <summary>
        ///     Removes the <see cref="IStumpResponseFactory"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void RemoveAt(int index);

        /// <summary>
        ///     Resets the next request to the start of the sequence regardless of the current position.
        /// </summary>
        void ResetToBeginning();
    }
}
