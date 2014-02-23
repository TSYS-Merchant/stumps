namespace Stumps.Proxy
{

    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     A class that determins if a block of text contains a specified value.
    /// </summary>
    internal sealed class TextContainsMatch
    {

        private readonly bool _ignoreCase;
        private readonly Regex _matchRegexValue;
        private readonly string _matchStringValue;
        private readonly bool _matchUsesRegex;
        private readonly bool _not;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.TextContainsMatch"/> class.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="ignoreCase">if set to <c>true</c>, capitalization is ignored.</param>
        public TextContainsMatch(string value, bool ignoreCase)
        {

            if (value.StartsWith(Resources.NotPattern, StringComparison.OrdinalIgnoreCase))
            {
                _not = true;
                value = value.Remove(0, Resources.NotPattern.Length);
            }

            if (value.StartsWith(Resources.RegExPattern, StringComparison.OrdinalIgnoreCase))
            {
                _matchUsesRegex = true;
                value = value.Remove(0, Resources.RegExPattern.Length);

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

            var match = _matchUsesRegex
                            ? _matchRegexValue.IsMatch(value)
                            : (value.IndexOf(_matchStringValue, 0, comparison) > -1);
            match = match ^ _not;

            return match;

        }

    }

}