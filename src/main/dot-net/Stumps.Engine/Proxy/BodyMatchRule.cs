namespace Stumps.Proxy {

    using System.Security.Cryptography;
    using Stumps.Http;
    using Stumps.Utility;

    internal sealed class BodyMatchRule : IStumpRule {

        private int _bodyLength;
        private byte[] _bodyHash;

        public BodyMatchRule(byte[] value) {

            _bodyLength = value.Length;

            using ( var hash = MD5.Create() ) {
                _bodyHash = hash.ComputeHash(value);
            }

        }

        public bool IsMatch(IStumpsHttpRequest request) {

            if ( request == null ) {
                return false;
            }

            var bodyBytes = StreamUtility.ConvertStreamToByteArray(request.InputStream);

            var match = false;

            if ( bodyBytes.Length == _bodyLength ) {

                using ( var hash = MD5.Create() ) {
                    var bytes = hash.ComputeHash(bodyBytes);
                    match = IsHashEqual(bytes);
                }

            }

            return match;

        }

        private bool IsHashEqual(byte[] hashBytes) {

            if ( hashBytes.Length != _bodyHash.Length ) {
                return false;
            }

            for ( var i = 0; i < _bodyHash.Length; i++ ) {
                if ( _bodyHash[i] != hashBytes[i] ) {
                    return false;
                }
            }

            return true;

        }

    }

}
