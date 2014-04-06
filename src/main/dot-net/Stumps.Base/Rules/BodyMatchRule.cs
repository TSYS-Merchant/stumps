namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the exact content of the body of an HTTP request.
    /// </summary>
    public class BodyMatchRule : IStumpRule
    {

        public const string BodySettingName = "body.base64";

        private byte[] _bodyValue;
        private byte[] _bodyHash;
        private int _bodyLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyMatchRule"/> class.
        /// </summary>
        public BodyMatchRule()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyMatchRule"/> class.
        /// </summary>
        /// <param name="value">The array of bytes matched against the HTTP requests's body.</param>
        public BodyMatchRule(byte[] value)
        {

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            InitializeRule(value);

        }

        public byte[] Body
        {
            get { return _bodyValue; }
        }

        /// <summary>
        ///     Gets the length of the body.
        /// </summary>
        /// <value>
        ///     The length of the body.
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
        ///     Gets an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </summary>
        /// <returns>
        ///     An enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </returns>
        public IEnumerable<RuleSetting> GetRuleSettings()
        {

            var settings = new[]
            {
                new RuleSetting { Name = BodyMatchRule.BodySettingName, Value = Convert.ToBase64String(_bodyValue, Base64FormattingOptions.None) }
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
            var bytes = helper.FindByteArray(BodyMatchRule.BodySettingName, new byte[0]);
            InitializeRule(bytes);

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
        /// <param name="value">The value used to initialize the rule.</param>
        private void InitializeRule(byte[] value)
        {

            _bodyLength = value.Length;
            _bodyValue = value;

            using (var hash = MD5.Create())
            {
                _bodyHash = hash.ComputeHash(value);
            }

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