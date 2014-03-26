namespace Stumps
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents a Stump.
    /// </summary>
    public sealed class Stump
    {

        private readonly List<IStumpRule> _ruleList;
        private IStumpsHttpResponse _response;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Stump" /> class.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the stump.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpId"/> is <c>null</c>, an empty string, or only contains white space.</exception>
        public Stump(string stumpId)
        {

            if (string.IsNullOrWhiteSpace(stumpId))
            {
                throw new ArgumentNullException("stumpId");
            }

            this.StumpId = stumpId;
            _ruleList = new List<IStumpRule>();

            _response = null;

        }

        /// <summary>
        ///     Gets or sets the response for the Stump.
        /// </summary>
        /// <value>
        ///     The response for the Stump.
        /// </value>
        public IStumpsHttpResponse Response
        {
            get
            {
                return _response;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _response = value;
            }
        }

        /// <summary>
        ///     Gets the number of rules for the Stump.
        /// </summary>
        /// <value>
        ///     The number of rules for the Stump.
        /// </value>
        public int Count
        {
            get { return _ruleList.Count; }
        }

        /// <summary>
        ///     Gets the unique identifier for the instance.
        /// </summary>
        /// <value>
        ///     The unique identifier for the instance.
        /// </value>
        public string StumpId
        {
            get;
            private set;
        }
        
        /// <summary>
        ///     Adds the rule to the Stump.
        /// </summary>
        /// <param name="rule">The rule to add to the Stump.</param>
        public void AddRule(IStumpRule rule)
        {

            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }

            _ruleList.Add(rule);

        }

        /// <summary>
        ///     Determines whether the context of the HTTP request is match for the current instance.
        /// </summary>
        /// <param name="context">The HTTP request.</param>
        /// <returns>
        ///     <c>true</c> if the instance matches the HTTP request; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpContext context)
        {

            if (context == null || _response == null || _ruleList.Count == 0)
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

    }

}