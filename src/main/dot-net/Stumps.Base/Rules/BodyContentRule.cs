namespace Stumps.Rules
{

    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///     A class representing a Stump rule that examines the content of the body using text evaluations 
    ///     for an HTTP request.
    /// </summary>
    public class BodyContentRule : IStumpRule
    {

        private readonly List<TextContainsMatch> _textMatchList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyContentRule"/> class.
        /// </summary>
        /// <param name="textEvaluators">The array of strings representing text evaluation rules.</param>
        public BodyContentRule(string[] textEvaluators)
        {

            if (textEvaluators == null)
            {
                return;
            }

            _textMatchList = new List<TextContainsMatch>(textEvaluators.Length);

            foreach (var rule in textEvaluators)
            {
                _textMatchList.Add(new TextContainsMatch(rule, false));
            }

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

            if (request == null || request.BodyLength == 0)
            {
                return false;
            }

            var buffer = request.GetBody();

            if (!TextAnalyzer.IsText(buffer))
            {
                return false;
            }

            var body = Encoding.UTF8.GetString(buffer);

            var match = true;

            foreach (var textMatch in _textMatchList)
            {
                match &= textMatch.IsMatch(body);

                if (!match)
                {
                    break;
                }
            }

            return match;

        }

    }

}