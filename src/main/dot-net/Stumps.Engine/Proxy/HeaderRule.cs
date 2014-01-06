namespace Stumps.Proxy {

    using Stumps.Http;

    internal class HeaderRule : IStumpRule {

        private readonly TextMatch _nameTextMatch;
        private readonly TextMatch _valueTextMatch;

        public HeaderRule(string name, string value) {

            name = name ?? string.Empty;
            value = value ?? string.Empty;

            _nameTextMatch = new TextMatch(name, true);
            _valueTextMatch = new TextMatch(value, true);

        }

        public bool IsMatch(IStumpsHttpRequest request) {

            if ( request == null || request.Headers == null ) {
                return false;
            }

            var match = false;

            foreach ( var headerName in request.Headers.AllKeys ) {

                var nameMatches = _nameTextMatch.IsMatch(headerName);

                if ( !nameMatches ) {
                    continue;
                }

                var valueMatches = _valueTextMatch.IsMatch(request.Headers[headerName]);

                if ( !valueMatches ) {
                    continue;
                }

                match = true;
                break;

            }

            return match;

        }

    }

}
