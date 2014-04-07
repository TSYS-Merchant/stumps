namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents the persisted form of a Stump rule.
    /// </summary>
    public class RuleEntity
    {

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
        ///     Gets or sets the settings used by the rule
        /// </summary>
        /// <value>
        ///     The settings used by the rule.
        /// </value>
        public IList<NameValuePairEntity> Settings
        {
            get;
            set;
        }

    }

}
