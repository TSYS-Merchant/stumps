namespace Stumps.Proxy {

    using System.Collections.Generic;
    using Stumps.Http;

    public sealed class Stump {

        private readonly List<IStumpRule> _ruleList;

        public Stump() {
            _ruleList = new List<IStumpRule>();
        }

        public StumpContract Contract { get; set; }

        public bool IsMatch(IStumpsHttpContext context) {

            var match = true;

            foreach ( var rule in _ruleList ) {
                match &= rule.IsMatch(context.Request);

                if ( !match ) {
                    match = false;
                    break;
                }
            }

            return match;

        }

        public void AddRule(IStumpRule rule) {
            _ruleList.Add(rule);
        }

    }

}
