namespace Stumps.Proxy {

    using Stumps.Http;

    internal class BodyLengthRule : IStumpRule {

        private readonly int _minimumValue;
        private readonly int _maximumSize;

        public BodyLengthRule(int minimumValue, int maximumValue) {
            _minimumValue = minimumValue;
            _maximumSize = maximumValue;
        }

        public bool IsMatch(IStumpsHttpRequest request) {

            if ( request == null ) {
                return false;
            }

            var match = (request.InputStream.Length >= _minimumValue && request.InputStream.Length <= _maximumSize);

            return match;

        }

    }

}
