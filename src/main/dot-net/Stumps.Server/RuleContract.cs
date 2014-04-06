namespace Stumps.Server
{

    using System.Collections.Generic;

    public class RuleContract
    {

        private readonly List<RuleSetting> _ruleSettings;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.RuleContract"/> class.
        /// </summary>
        public RuleContract()
        {
            _ruleSettings = new List<RuleSetting>();
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
        ///     Appends a <see cref="T:Stumps.RuleSetting"/> to the contract.
        /// </summary>
        /// <param name="setting">The <see cref="T:Stumps.RuleSetting"/> to add to the contract.</param>
        public void AppendRuleSetting(RuleSetting setting)
        {

            if (setting == null || string.IsNullOrWhiteSpace(setting.Name) || setting.Value == null)
            {
                return;
            }

            _ruleSettings.Add(setting);

        }

        /// <summary>
        ///     Gets an array of the <see cref="T:Stumps.RuleSetting" /> objects for the contract.
        /// </summary>
        /// <returns>An array of <see cref="T:Stumps.RuleSetting"/> objects.</returns>
        public RuleSetting[] GetRuleSettings()
        {
            return _ruleSettings.ToArray();
        }

    }

}
