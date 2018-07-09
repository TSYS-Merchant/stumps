namespace Stumps.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     A helper class that provides a translation between contracts and Stump objects.
    /// </summary>
    public static class ContractBindings
    {
        private static readonly Dictionary<string, Type> KnownRules = FindRulesForAssembly();

        /// <summary>
        ///     Creates a Stump from a contract.
        /// </summary>
        /// <param name="contract">The <see cref="StumpContract"/> used to create the Stump.</param>
        /// <returns>
        ///     A <see cref="Stump"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="contract"/> is <c>null</c>.</exception>
        public static Stump CreateStumpFromContract(StumpContract contract)
        {
            contract = contract ?? throw new ArgumentNullException(nameof(contract));

            var stump = new Stump(contract.StumpId);

            foreach (var r in contract.Rules)
            {
                var rule = ContractBindings.CreateRuleFromContract(r);
                stump.AddRule(rule);
            }

            stump.Responses.Add(contract.Response);

            return stump;
        }

        /// <summary>
        ///     Creates a <see cref="IStumpRule"/> from a <see cref="RuleContract"/>.
        /// </summary>
        /// <param name="contract">The <see cref="RuleContract"/> used to create the <see cref="IStumpRule"/>.</param>
        /// <returns>A <see cref="IStumpRule"/> object created from the specified <paramref name="contract"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contract"/> is <c>null</c>.</exception>
        public static IStumpRule CreateRuleFromContract(RuleContract contract)
        {
            contract = contract ?? throw new ArgumentNullException(nameof(contract));

            var type = KnownRules[contract.RuleName];
            var rule = Activator.CreateInstance(type) as IStumpRule;
            rule.InitializeFromSettings(contract.GetRuleSettings());
            return rule;
        }

        /// <summary>
        ///     Creates an object based on an <see cref="IStumpRule"/> from a <see cref="RuleContract"/>.
        /// </summary>
        /// <typeparam name="T">The concrete implementation of the <see cref="IStumpRule"/> rule to create.</typeparam>
        /// <param name="contract">The <see cref="RuleContract"/> used to create the <see cref="IStumpRule"/>.</param>
        /// <returns>A <see cref="IStumpRule"/> object created from the specified <paramref name="contract"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="contract"/> is <c>null</c>.</exception>
        public static T CreateRuleFromContract<T>(RuleContract contract) where T : IStumpRule, new()
        {
            contract = contract ?? throw new ArgumentNullException(nameof(contract));

            var type = KnownRules[contract.RuleName];
            var rule = Activator.CreateInstance(type) as IStumpRule;

            if (rule is T)
            {
                rule.InitializeFromSettings(contract.GetRuleSettings());
            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T));
            }

            return (T)rule;
        }

        /// <summary>
        /// Finds the rules for the current assembly.
        /// </summary>
        /// <returns>A dictionary of rules for the current assembly.</returns>
        private static Dictionary<string, Type> FindRulesForAssembly()
        {
            var dict = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var baseType = typeof(IStumpRule);

            var assembly = typeof(StumpsServer).Assembly;
            var types = assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
            
            foreach (var t in types)
            {
                dict.Add(t.Name, t);
            }

            return dict;
        }
    }
}
