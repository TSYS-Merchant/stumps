namespace Stumps
{
    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents a Stumps rule when attempting to match against an <see cref="IStumpsHttpRequest"/>.
    /// </summary>
    public interface IStumpRule
    {
        /// <summary>
        /// Gets a value indicating whether the rule is initialized.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the rule is initialized; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        ///     Gets an enumerable list of <see cref="RuleSetting"/> objects used to represent the current instance.
        /// </summary>
        /// <returns>An enumerable list of <see cref="RuleSetting"/> objects used to represent the current instance.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Property is not appropriate.")]
        IEnumerable<RuleSetting> GetRuleSettings();

        /// <summary>
        ///     Initializes a rule from an enumerable list of <see cref="RuleSetting"/> objects.
        /// </summary>
        /// <param name="settings">The enumerable list of <see cref="RuleSetting"/> objects.</param>
        void InitializeFromSettings(IEnumerable<RuleSetting> settings);

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="IStumpsHttpRequest"/> to evaluate.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="request"/> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch(IStumpsHttpRequest request);
    }
}
