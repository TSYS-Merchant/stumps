namespace Stumps.Proxy
{

    using System.Collections.Generic;
    using Stumps.Http;

    /// <summary>
    ///     A class that represents a Stump.
    /// </summary>
    public sealed class Stump
    {

        private readonly List<IStumpRule> _ruleList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.Stump"/> class.
        /// </summary>
        public Stump()
        {
            _ruleList = new List<IStumpRule>();
        }

        /// <summary>
        ///     Gets or sets the contract of the Stump.
        /// </summary>
        /// <value>
        ///     The contract of the Stump.
        /// </value>
        public StumpContract Contract { get; set; }

        /// <summary>
        ///     Determines whether the context of the HTTP request is match for the current instance.
        /// </summary>
        /// <param name="context">The HTTP request.</param>
        /// <returns>
        ///     <c>true</c> if the instance matches the HTTP request; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpContext context)
        {

            if (context == null)
            {
                return false;
            }

            var match = true;

            foreach (var rule in _ruleList)
            {
                match &= rule.IsMatch(context.Request);

                if (!match)
                {
                    break;
                }
            }

            return match;

        }

        /// <summary>
        ///     Adds the rule to the Stump.
        /// </summary>
        /// <param name="rule">The rule to add to the Stump.</param>
        public void AddRule(IStumpRule rule)
        {
            _ruleList.Add(rule);
        }

    }

}