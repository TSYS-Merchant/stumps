namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the URL of an HTTP request.
    /// </summary>
    public class UrlRule : IStumpRule
    {

        private const string UrlSetting = "url.value";

        private TextMatch _textMatch;
        private string _textMatchValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.UrlRule"/> class.
        /// </summary>
        public UrlRule()
        {   
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.UrlRule"/> class.
        /// </summary>
        /// <param name="value">The value used for the URL rule.</param>
        public UrlRule(string value)
        {
            InitializeRule(value);
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
        ///     Gets the text match rule for the URL.
        /// </summary>
        /// <value>
        ///     The text match rule for the URL.
        /// </value>
        public string UrlTextMatch
        {
            get { return _textMatchValue; }
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
                new RuleSetting { Name = UrlRule.UrlSetting, Value = _textMatchValue }
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
            var value = helper.FindString(UrlRule.UrlSetting, string.Empty);

            InitializeRule(value);

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

            var match = _textMatch.IsMatch(request.RawUrl);
            return match;

        }

        /// <summary>
        ///     Initializes the rule.
        /// </summary>
        /// <param name="value">The value used for the URL rule.</param>
        public void InitializeRule(string value)
        {
            _textMatchValue = value ?? string.Empty;

            _textMatch = new TextMatch(_textMatchValue, true);
            this.IsInitialized = true;

        }

    }

}