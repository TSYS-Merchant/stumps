namespace Stumps
{
    /// <summary>
    ///     An interface that represents a factory capable of creating or returning an HTTP response given an HTTP request.
    /// </summary>
    public interface IStumpsHttpResponseFactory
    {
        /// <summary>
        /// Gets a value indicating whether this instance has a valid response available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a valid response available; otherwise, <c>false</c>.
        /// </value>
        bool HasResponse { get; }

        /// <summary>
        ///     Creates an <see cref="T:Stumps.IStumpsHttpResponse"/> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest"/>
        ///     which is returned for a Stump positivly matching all necessary criteria.
        /// </summary>
        /// <param name="request">The incoming <see cref="T:Stumps.IStumpsHttpRequest"/>.</param>
        /// <returns>An <see cref="T:Stumps.IStumpsHttpResponse"/> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest"/>.</returns>
        IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request);
    }
}
