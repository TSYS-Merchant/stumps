namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     A helper class that provides a translation between contracts and Stump objects.
    /// </summary>
    internal static class ContractBindings
    {

        /// <summary>
        ///     Creates a Stump from a contract.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.StumpContract"/> used to create the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        public static Stump CreateStumpFromContract(StumpContract contract)
        {

            var stump = new Stump(contract.StumpId);

            foreach (var r in contract.Rules)
            {
                var rule = ContractBindings.CreateRuleFromContract(r);
                stump.AddRule(rule);
            }

            stump.Response = contract.Response;

            return stump;

        }

        /// <summary>
        ///     Creates a <see cref="T:Stumps.IStumpRule"/> from a <see cref="T:Stumps.Server.RuleContract"/>.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.RuleContract"/> used to create the <see cref="T:Stumps.IStumpRule"/>.</param>
        /// <returns>A <see cref="T:Stumps.IStumpRule"/> object created from the specified <paramref name="contract"/>.</returns>
        public static IStumpRule CreateRuleFromContract(RuleContract contract)
        {

            var assembly = typeof(StumpsServer).Assembly;
            var type = assembly.GetType(contract.RuleName);
            var rule = Activator.CreateInstance(type) as IStumpRule;
            return rule;

        }

        /// <summary>
        ///     Creates an object based on an <see cref="T:Stumps.IStumpRule"/> from a <see cref="T:Stumps.Server.RuleContract"/>.
        /// </summary>
        /// <typeparam name="T">The concrete implementation of the <see cref="T:Stumps.IStumpRule"/> rule to create.</typeparam>
        /// <param name="contract">The <see cref="T:Stumps.Server.RuleContract"/> used to create the <see cref="T:Stumps.IStumpRule"/>.</param>
        /// <returns>A <see cref="T:Stumps.IStumpRule"/> object created from the specified <paramref name="contract"/>.</returns>
        public static T CreateRuleFromContract<T>(RuleContract contract) where T : IStumpRule, new()
        {

            var assembly = typeof(StumpsServer).Assembly;
            var type = assembly.GetType(contract.RuleName);
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

    }

}
