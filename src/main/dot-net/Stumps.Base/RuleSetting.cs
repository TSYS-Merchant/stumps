namespace Stumps
{
    /// <summary>
    ///     A class that defines a setting for a <see cref="T:Stumps.IStumpRule"/>.
    /// </summary>
    public class RuleSetting
    {
        /// <summary>
        ///     Gets or sets the name of the setting for the rule.
        /// </summary>
        /// <value>
        ///     The name of the setting for the rule.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the value of the setting.
        /// </summary>
        /// <value>
        ///     The value of the setting.
        /// </value>
        public string Value
        {
            get;
            set;
        }
    }
}
