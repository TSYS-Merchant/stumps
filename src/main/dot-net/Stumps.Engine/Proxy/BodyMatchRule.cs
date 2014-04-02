﻿namespace Stumps.Proxy
{

    using System.Security.Cryptography;
    using Stumps.Http;
    using Stumps.Utility;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the exact content of the body of an HTTP request.
    /// </summary>
    internal sealed class BodyMatchRule : IStumpRule
    {

        private readonly byte[] _bodyHash;
        private readonly int _bodyLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.BodyMatchRule"/> class.
        /// </summary>
        /// <param name="value">The array of bytes matched against the HTTP requests's body.</param>
        public BodyMatchRule(byte[] value)
        {

            _bodyLength = value.Length;

            using (var hash = MD5.Create())
            {
                _bodyHash = hash.ComputeHash(value);
            }

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

            var bodyBytes = StreamUtility.ConvertStreamToByteArray(request.InputStream);

            var match = false;

            if (bodyBytes.Length == _bodyLength)
            {

                using (var hash = MD5.Create())
                {
                    var bytes = hash.ComputeHash(bodyBytes);
                    match = IsHashEqual(bytes);
                }

            }

            return match;

        }

        /// <summary>
        ///     Determines whether the bytes in a specified hash match the hash for the rule.
        /// </summary>
        /// <param name="hashBytes">The hash bytes computed for the HTTP requests's body.</param>
        /// <returns>
        ///     <c>true</c> if the hashes are equal; otherwise, <c>false</c>.
        /// </returns>
        private bool IsHashEqual(byte[] hashBytes)
        {

            if (hashBytes.Length != _bodyHash.Length)
            {
                return false;
            }

            for (var i = 0; i < _bodyHash.Length; i++)
            {
                if (_bodyHash[i] != hashBytes[i])
                {
                    return false;
                }
            }

            return true;

        }

    }

}