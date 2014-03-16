namespace Stumps.Rules
{

    /// <summary>
    ///     A class representing a Stump rule that evaluates the length of the body of an HTTP request.
    /// </summary>
    internal class BodyLengthRule : IStumpRule
    {

        private readonly int _maximumSize;
        private readonly int _minimumValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyLengthRule"/> class.
        /// </summary>
        /// <param name="minimumBodyValue">The length of the body.</param>
        /// <param name="maximumBodyLength">The maximum length of the body.</param>
        public BodyLengthRule(int minimumBodyValue, int maximumBodyLength)
        {
            _minimumValue = minimumBodyValue;
            _maximumSize = maximumBodyLength;
        }

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="T:Stumps.IStumpsHttpRequest" /> to evaluate.</param>
        /// <returns>
        ///   <c>true</c> if the <paramref name="request" /> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        public bool IsMatch(IStumpsHttpRequest request)
        {

            if (request == null)
            {
                return false;
            }

            var match = request.BodyLength >= _minimumValue && request.BodyLength <= _maximumSize;

            return match;

        }

    }

}