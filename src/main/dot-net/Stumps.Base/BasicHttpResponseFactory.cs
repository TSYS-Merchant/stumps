namespace Stumps
{
    using System;

    /// <summary>
    ///     A simple implementation of the <see cref="IStumpsHttpResponseFactory"/> which can only return a single static response.
    /// </summary>
    /// <seealso cref="Stumps.IStumpsHttpResponseFactory" />
    public class BasicHttpResponseFactory : IStumpsHttpResponseFactory
    {
        public BasicHttpResponseFactory()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicHttpResponseFactory"/> class.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.IStumpsHttpResponse"/> returned by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public BasicHttpResponseFactory(IStumpsHttpResponse response)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        /// <summary>
        ///     Gets or sets the <see cref="T:Stumps.IStumpsHttpResponse"/> returned by the factory.
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
        /// Creates an <see cref="T:Stumps.IStumpsHttpResponse" /> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest" />
        /// which is returned for a Stump positivly matching all necessary criteria.
        /// </summary>
        /// <param name="request">The incoming <see cref="T:Stumps.IStumpsHttpRequest" />.</param>
        /// <returns>
        /// An <see cref="T:Stumps.IStumpsHttpResponse" /> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest" />.
        /// </returns>
        public IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request)
        {
            return this.Response;
        }
    }
}
