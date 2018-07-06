namespace Stumps
{
    using System;

    /// <summary>
    ///     A simple implementation of the <see cref="IStumpsHttpResponseFactory"/> which can only return a single response.
    /// </summary>
    /// <seealso cref="IStumpsHttpResponseFactory" />
    public class SingleHttpResponseFactory : IStumpsHttpResponseFactory
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleHttpResponseFactory"/> class.
        /// </summary>
        public SingleHttpResponseFactory()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingleHttpResponseFactory"/> class.
        /// </summary>
        /// <param name="response">The <see cref="IStumpsHttpResponse"/> returned by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public SingleHttpResponseFactory(IStumpsHttpResponse response)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has a valid response available.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has a valid response available; otherwise, <c>false</c>.
        /// </value>
        public bool HasResponse => this.Response != null;

        /// <summary>
        ///     Gets or sets the <see cref="IStumpsHttpResponse"/> returned by the factory.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.IStumpsHttpResponse"/> returned by the factory.
        /// </value>
        public IStumpsHttpResponse Response
        {
            get;
            set;
        }

        /// <summary>
        /// Creates an <see cref="IStumpsHttpResponse" /> object based on an incoming <see cref="IStumpsHttpRequest" />
        /// which is returned for a Stump positivly matching all necessary criteria.
        /// </summary>
        /// <param name="request">The incoming <see cref="IStumpsHttpRequest" />.</param>
        /// <returns>
        /// An <see cref="IStumpsHttpResponse" /> object based on an incoming <see cref="IStumpsHttpRequest" />.
        /// </returns>
        public IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request)
        {
            return this.Response;
        }
    }
}
