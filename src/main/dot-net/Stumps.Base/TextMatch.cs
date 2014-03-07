namespace Stumps
{

    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     A class that provides text matching for various rules.
    /// </summary>
    internal sealed class TextMatch
    {

        private readonly bool _ignoreCase;
        private readonly Regex _matchRegexValue;
        private readonly string _matchStringValue;
        private readonly bool _matchUsesRegex;
        private readonly bool _not;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.TextMatch"/> class.
        /// </summary>
        /// <param name="value">The value used to match.</param>
        /// <param name="ignoreCase">If set to <c>true</c>, capitalization is ignored.</param>
        public TextMatch(string value, bool ignoreCase)
        {

            if (value.StartsWith(BaseResources.NotPattern, StringComparison.OrdinalIgnoreCase))
            {
                _not = true;
                value = value.Remove(0, BaseResources.NotPattern.Length);
            }

            if (value.StartsWith(BaseResources.RegExPattern, StringComparison.OrdinalIgnoreCase))
            {
                _matchUsesRegex = true;
                value = value.Remove(0, BaseResources.RegExPattern.Length);

                if (ignoreCase)
                {
                    _matchRegexValue = new Regex(value, RegexOptions.IgnoreCase);
                }
                else
                {
                    _matchRegexValue = new Regex(value);
                }

            }
            else
            {
                _matchStringValue = value;
            }

            _ignoreCase = ignoreCase;

        }

        /// <summary>
        ///     Determines whether the specified value matches the text.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns><c>true</c> if the text matches; otherwise, <c>false</c>.</returns>
        public bool IsMatch(string value)
        {

            var comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            var match = _matchUsesRegex ? _matchRegexValue.IsMatch(value) : _matchStringValue.Equals(value, comparison);
            match = match ^ _not;

            return match;

        }

    }

}