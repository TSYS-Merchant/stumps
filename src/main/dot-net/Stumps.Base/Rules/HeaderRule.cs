namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class representing a Stump rule that evaluates the headers of an HTTP request.
    /// </summary>
    public class HeaderRule : IStumpRule
    {

        public const string HeaderNameSetting = "header.name";
        public const string HeaderValueSetting = "header.value";

        private string _headerNameValue;
        private string _headerValueValue;

        private TextMatch _nameTextMatch;
        private TextMatch _valueTextMatch;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.HeaderRule"/> class.
        /// </summary>
        public HeaderRule()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.HeaderRule"/> class.
        /// </summary>
        /// <param name="name">The name of the HTTP header.</param>
        /// <param name="value">The value of the HTTP header.</param>
        public HeaderRule(string name, string value)
        {

            InitializeRule(name, value);

        }

        /// <summary>
        ///     Gets the text match rule for the header name.
        /// </summary>
        /// <value>
        ///     The text match rule for the header name.
        /// </value>
        public string HeaderNameTextMatch
        {
            get { return _headerNameValue; }
        }

        /// <summary>
        ///     Gets the text match rule for the header value.
        /// </summary>
        /// <value>
        ///     The text match rule for the header value.
        /// </value>
        public string HeaderValueTextMatch
        {
            get { return _headerValueValue; }
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
                new RuleSetting { Name = HeaderRule.HeaderNameSetting, Value = _headerNameValue },
                new RuleSetting { Name = HeaderRule.HeaderValueSetting, Value = _headerValueValue }
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
            var name = helper.FindString(HeaderRule.HeaderNameSetting, string.Empty);
            var value = helper.FindString(HeaderRule.HeaderValueSetting, string.Empty);

            InitializeRule(name, value);

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

        /// <summary>
        ///     Initializes the rule.
        /// </summary>
        /// <param name="name">The name of the HTTP header.</param>
        /// <param name="value">The value of the HTTP header.</param>
        private void InitializeRule(string name, string value)
        {
            name = name ?? string.Empty;
            value = value ?? string.Empty;

            _headerNameValue = name;
            _headerValueValue = value;

            _nameTextMatch = new TextMatch(name, true);
            _valueTextMatch = new TextMatch(value, true);

            this.IsInitialized = true;
        }

    }

}