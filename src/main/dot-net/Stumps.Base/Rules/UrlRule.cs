namespace Stumps.Rules
{

    using Stumps.Http;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the URL of an HTTP request.
    /// </summary>
    internal class UrlRule : IStumpRule
    {

        private readonly TextMatch _textMatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.UrlRule"/> class.
        /// </summary>
        /// <param name="value">The value used for the URL rule.</param>
        public UrlRule(string value)
        {

            value = value ?? string.Empty;

            _textMatch = new TextMatch(value, true);

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

            var match = _textMatch.IsMatch(request.RawUrl);
            return match;

        }

    }

}