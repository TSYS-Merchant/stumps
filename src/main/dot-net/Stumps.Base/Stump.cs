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
        private IStumpsHttpResponseFactory _responseFactory;

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

            _responseFactory = null;
        }

        /// <summary>
        ///     Gets the number of rules for the Stump.
        /// </summary>
        /// <value>
        ///     The number of rules for the Stump.
        /// </value>
        public int Count => _ruleList.Count;

        /// <summary>
        ///     Gets or sets the response for the Stump.
        /// </summary>
        /// <value>
        ///     The response for the Stump.
        /// </value>
        public IStumpsHttpResponseFactory ResponseFactory
        {
            get => _responseFactory;
            set => _responseFactory = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        ///     Gets or sets the amount of time (in milliseconds) the response is delayed.
        /// </summary>
        /// <value>
        ///     The amount of time (in milliseconds) the response is delayed.
        /// </value>
        /// <remarks>A value of <c>0</c> or less will not cause a delay.</remarks>
        public int ResponseDelay
        {
            get;
            set;
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
        ///     Gets or sets a flag indicating whether to forceably terminate the connection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the connection should be forceably terminated; otherwise, <c>false</c>.
        /// </value>
        public bool TerminateConnection
        {
            get;
            set;
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