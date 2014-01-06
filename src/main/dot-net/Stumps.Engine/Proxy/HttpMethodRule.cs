namespace Stumps.Proxy {

    using Stumps.Http;

    internal class HttpMethodRule : IStumpRule {

        private readonly TextMatch _textMatch;

        public HttpMethodRule(string value) {

            value = value ?? string.Empty;

            _textMatch = new TextMatch(value, true);

        }

        public bool IsMatch(IStumpsHttpRequest request) {

            if ( request == null ) {
                return false;
            }

            var match = _textMatch.IsMatch(request.HttpMethod);
            return match;

        }

    }

}
