namespace Stumps.Rules
{

    using Stumps.Http;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the headers of an HTTP request.
    /// </summary>
    internal class HeaderRule : IStumpRule
    {

        private readonly TextMatch _nameTextMatch;
        private readonly TextMatch _valueTextMatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.HeaderRule"/> class.
        /// </summary>
        /// <param name="name">The name of the HTTP header.</param>
        /// <param name="value">The value of the HTTP header.</param>
        public HeaderRule(string name, string value)
        {

            name = name ?? string.Empty;
            value = value ?? string.Empty;

            _nameTextMatch = new TextMatch(name, true);
            _valueTextMatch = new TextMatch(value, true);

        }

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="T:Stumps.IStumpsHttpRequest" /> to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if the <paramref name="request" /> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpRequest request)
        {

            if (request == null || request.Headers == null)
            {
                return false;
            }

            var match = false;

            foreach (var headerName in request.Headers.HeaderNames)
            {

                var nameMatches = _nameTextMatch.IsMatch(headerName);

                if (!nameMatches)
                {
                    continue;
                }

                var valueMatches = _valueTextMatch.IsMatch(request.Headers[headerName]);

                if (!valueMatches)
                {
                    continue;
                }

                match = true;
                break;

            }

            return match;

        }

    }

}