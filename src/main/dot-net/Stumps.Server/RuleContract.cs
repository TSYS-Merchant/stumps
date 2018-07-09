namespace Stumps.Server
{
    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents a contract for a rule.
    /// </summary>
    public class RuleContract
    {
        private readonly List<RuleSetting> _ruleSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RuleContract"/> class.
        /// </summary>
        public RuleContract()
        {
            _ruleSettings = new List<RuleSetting>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RuleContract"/> class.
        /// </summary>
        /// <param name="rule">The <see cref="IStumpRule"/> used to create the instance.</param>
        public RuleContract(IStumpRule rule) : this()
        {
            if (rule == null)
            {
                return;
            }

            this.RuleName = rule.GetType().Name;
            var settings = rule.GetRuleSettings();

            foreach (var setting in settings)
            {
                _ruleSettings.Add(setting);
            }
        }

        /// <summary>
        ///     Gets or sets the name of the rule.
        /// </summary>
        /// <value>
        ///     The name of the rule.
        /// </value>
        public string RuleName
        {
            get;
            set;
        }

        /// <summary>
        ///     Appends a <see cref="RuleSetting"/> to the contract.
        /// </summary>
        /// <param name="setting">The <see cref="RuleSetting"/> to add to the contract.</param>
        public void AppendRuleSetting(RuleSetting setting)
        {
            if (setting == null || string.IsNullOrWhiteSpace(setting.Name) || setting.Value == null)
            {
                return;
            }

            _ruleSettings.Add(setting);
        }

        /// <summary>
        ///     Gets an array of the <see cref="RuleSetting" /> objects for the contract.
        /// </summary>
        /// <returns>An array of <see cref="RuleSetting"/> objects.</returns>
        public RuleSetting[] GetRuleSettings() => _ruleSettings.ToArray();
    }
}
