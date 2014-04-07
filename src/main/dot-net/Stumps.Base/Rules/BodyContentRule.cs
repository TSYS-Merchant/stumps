namespace Stumps.Rules
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///     A class representing a Stump rule that examines the content of the body using text evaluations 
    ///     for an HTTP request.
    /// </summary>
    public class BodyContentRule : IStumpRule
    {

        private const string TextEvaluationSettingName = "text.evaluation";

        private List<TextContainsMatch> _textMatchList;
        private string[] _textMatches;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyContentRule"/> class.
        /// </summary>
        public BodyContentRule()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Rules.BodyContentRule"/> class.
        /// </summary>
        /// <param name="textEvaluators">The array of strings representing text evaluation rules.</param>
        public BodyContentRule(string[] textEvaluators)
        {

            InitializeRule(textEvaluators);

        }

        /// <summary>
        ///     Gets a value indicating whether the rule is initialized.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the rule is initialized; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitialized
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </summary>
        /// <returns>
        ///     An enumerable list of <see cref="T:Stumps.RuleSetting" /> objects used to represent the current instance.
        /// </returns>
        public IEnumerable<RuleSetting> GetRuleSettings()
        {

            var settings = new List<RuleSetting>();

            if (_textMatches != null)
            {
                settings.AddRange(this._textMatches.Select(s => new RuleSetting()
                {
                    Name = BodyContentRule.TextEvaluationSettingName,
                    Value = s
                }));
            }

            return settings;

        }

        /// <summary>
        ///     Gets the text evaluators.
        /// </summary>
        /// <value>
        ///     The text evaluators used for the body content rule.
        /// </value>
        public string[] GetTextEvaluators()
        {
            return _textMatches;
        }

        /// <summary>
        ///     Initializes a rule from an enumerable list of <see cref="T:Stumps.RuleSetting" /> objects.
        /// </summary>
        /// <param name="settings">The enumerable list of <see cref="T:Stumps.RuleSetting" /> objects.</param>
        public void InitializeFromSettings(IEnumerable<RuleSetting> settings)
        {

            if (this.IsInitialized)
            {
                throw new InvalidOperationException(BaseResources.BodyRuleAlreadyInitializedError);
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var textEvaluators = new List<string>();

            foreach (var setting in settings)
            {
                if (setting.Value != null &&
                    setting.Name != null &&
                    setting.Name.Equals(BodyContentRule.TextEvaluationSettingName, StringComparison.OrdinalIgnoreCase))
                {
                    textEvaluators.Add(setting.Value);
                }
            }

            InitializeRule(textEvaluators.ToArray());

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

        /// <summary>
        ///     Initializes the rule.
        /// </summary>
        /// <param name="textEvaluators">The array of strings representing text evaluation rules.</param>
        private void InitializeRule(string[] textEvaluators)
        {

            this.IsInitialized = true;

            _textMatches = textEvaluators;

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

    }

}