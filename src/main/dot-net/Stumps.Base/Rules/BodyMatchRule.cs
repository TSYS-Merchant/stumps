namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the exact content of the body of an HTTP request.
    /// </summary>
    public class BodyMatchRule : IStumpRule
    {

        private const string BodyLengthSetting = "body.length";
        private const string BodyMd5HashSetting = "body.md5";

        private byte[] _bodyHash;
        private string _bodyHashString;
        private int _bodyLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyMatchRule"/> class.
        /// </summary>
        public BodyMatchRule()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyMatchRule" /> class.
        /// </summary>
        /// <param name="length">The expected length of the body.</param>
        /// <param name="md5Hash">The MD5 hash of the body.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="length"/> must be equal or greater than 0.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="md5Hash" /> is <c>null</c>.</exception>
        public BodyMatchRule(int length, string md5Hash)
        {

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (md5Hash == null)
            {
                throw new ArgumentNullException("md5Hash");
            }

            InitializeRule(length, md5Hash);

        }

        /// <summary>
        ///     Gets the expected length of the body.
        /// </summary>
        /// <value>
        ///     The expected length of the body.
        /// </value>
        public int BodyLength
        {
            get { return _bodyLength; }
        }
        
        /// <summary>
        ///     Gets a value indicating whether the rule is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the rule is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the expected MD5 hash of the body.
        /// </summary>
        /// <value>
        ///     The expected MD5 hash of the body.
        /// </value>
        public string Md5Hash
        {
            get { return _bodyHashString; }
        }

        /// <summary>
        ///     Gets an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </summary>
        /// <returns>
        ///     An enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </returns>
        public IEnumerable<RuleSetting> GetRuleSettings()
        {

            var settings = new[]
            {
                new RuleSetting { Name = BodyMatchRule.BodyLengthSetting, Value = _bodyLength.ToString(CultureInfo.InvariantCulture) },
                new RuleSetting { Name = BodyMatchRule.BodyMd5HashSetting, Value = _bodyHashString }
            };

            return settings;

        }

        /// <summary>
        ///     Initializes a rule from an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects.
        /// </summary>
        /// <param name="settings">The enumerable list of <see cref="T:Stumps.RuleSetting" /> objects.</param>
        public void InitializeFromSettings(IEnumerable<RuleSetting> settings)
        {

            if (this.IsInitialized)
            {
                throw new InvalidOperationException(BaseResources.BodyRuleAlreadyInitializedError);
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var helper = new RuleSettingsHelper(settings);
            var length = helper.FindInteger(BodyMatchRule.BodyLengthSetting, 0);
            var md5Hash = helper.FindString(BodyMatchRule.BodyMd5HashSetting, "000000");

            InitializeRule(length, md5Hash);

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

            if (request == null)
            {
                return false;
            }

            var match = false;

            if (request.BodyLength == _bodyLength)
            {

                using (var hash = MD5.Create())
                {
                    var bytes = hash.ComputeHash(request.GetBody());
                    match = IsHashEqual(bytes);
                }

            }

            return match;

        }

        /// <summary>
        ///     Initializes the rule.
        /// </summary>
        /// <param name="length">The expected length of the body.</param>
        /// <param name="md5Hash">The MD5 hash of the body.</param>
        private void InitializeRule(int length, string md5Hash)
        {

            _bodyLength = length;
            _bodyHashString = md5Hash;
            _bodyHash = md5Hash.ToByteArray();

            this.IsInitialized = true;

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