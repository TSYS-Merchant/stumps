namespace Stumps.Proxy
{

    using Stumps.Http;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the HTTP method of an HTTP request.
    /// </summary>
    internal class HttpMethodRule : IStumpRule
    {

        private readonly TextMatch _textMatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.HttpMethodRule"/> class.
        /// </summary>
        /// <param name="httpMethod">The HTTP method for the rule.</param>
        public HttpMethodRule(string httpMethod)
        {

            httpMethod = httpMethod ?? string.Empty;

            _textMatch = new TextMatch(httpMethod, true);

        }

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="T:Stumps.Http.IStumpsHttpRequest" /> to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if the <paramref name="request" /> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpRequest request)
        {

            if (request == null)
            {
                return false;
            }

            var match = _textMatch.IsMatch(request.HttpMethod);
            return match;

        }

    }

}