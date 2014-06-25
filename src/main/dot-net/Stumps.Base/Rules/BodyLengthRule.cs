namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the length of the body of an HTTP request.
    /// </summary>
    public class BodyLengthRule : IStumpRule
    {

        private const string MaximumLengthSettingName = "length.maximum";
        private const string MinimumLengthSettingName = "length.minimum";

        private const int DefaultMinimumLength = int.MinValue;
        private const int DefaultMaximumLength = int.MinValue;

        private int _maximumLength;
        private int _minimumLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyLengthRule"/> class.
        /// </summary>
        public BodyLengthRule()
        {
            _maximumLength = BodyLengthRule.DefaultMaximumLength;
            _minimumLength = BodyLengthRule.DefaultMinimumLength;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyLengthRule"/> class.
        /// </summary>
        /// <param name="minimumBodyLength">The minimum length of the body.</param>
        /// <param name="maximumBodyLength">The maximum length of the body.</param>
        public BodyLengthRule(int minimumBodyLength, int maximumBodyLength)
        {

            this.IsInitialized = true;
            _minimumLength = minimumBodyLength;
            _maximumLength = maximumBodyLength;

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
        ///     Gets the maximum length of the body.
        /// </summary>
        /// <value>
        ///     The maximum length of the body.
        /// </value>
        public int MaximumBodyLength
        {
            get { return _maximumLength; }
        }

        /// <summary>
        ///     Gets the minimum length of the body.
        /// </summary>
        /// <value>
        ///     The minimum length of the body.
        /// </value>
        public int MinimumBodyLength
        {
            get { return _minimumLength; }
        }
        
        /// <summary>
        ///     Gets an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </summary>
        /// <returns>
        ///     An enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </returns>
        public IEnumerable<RuleSetting> GetRuleSettings()
        {

            var helper = new RuleSettingsHelper();
            helper.Add(BodyLengthRule.MaximumLengthSettingName, _maximumLength);
            helper.Add(BodyLengthRule.MinimumLengthSettingName, _minimumLength);

            return helper.ToEnumerableList();

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
            _maximumLength = helper.FindInteger(BodyLengthRule.MaximumLengthSettingName, BodyLengthRule.DefaultMaximumLength);
            _minimumLength = helper.FindInteger(BodyLengthRule.MinimumLengthSettingName, BodyLengthRule.DefaultMinimumLength);

            this.IsInitialized = true;

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

            var match = request.BodyLength >= _minimumLength && request.BodyLength <= _maximumLength;

            return match;

        }
        
    }

}