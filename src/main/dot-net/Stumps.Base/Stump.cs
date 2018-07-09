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
        private IStumpResponseFactory _responseFactory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Stump" /> class.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the stump.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpId"/> is <c>null</c>, an empty string, or only contains white space.</exception>
        public Stump(string stumpId)
        {
            if (string.IsNullOrWhiteSpace(stumpId))
            {
                throw new ArgumentNullException(nameof(stumpId));
            }

            this.StumpId = stumpId;
            _ruleList = new List<IStumpRule>();

            _responseFactory = new StumpResponseFactory();
        }

        /// <summary>
        ///     Gets the number of rules for the Stump.
        /// </summary>
        /// <value>
        ///     The number of rules for the Stump.
        /// </value>
        public int Count => _ruleList.Count;

        /// <summary>
        ///     Gets or sets the response factory for the stump.
        /// </summary>
        /// <value>
        ///     The response factory for the stump.
        /// </value>
        public IStumpResponseFactory Responses
        {
            get => _responseFactory;
            set => _responseFactory = value ?? throw new ArgumentNullException(nameof(value));
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
            rule = rule ?? throw new ArgumentNullException(nameof(rule));

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
            if (context == null 
                || _responseFactory?.HasResponse == false
                || _ruleList.Count == 0)
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