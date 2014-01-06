namespace Stumps.Proxy {

    using System;
    using System.Text.RegularExpressions;

    internal sealed class TextContainsMatch {

        private readonly bool _not;
        private readonly bool _matchUsesRegex;
        private readonly Regex _matchRegexValue;
        private readonly string _matchStringValue;
        private readonly bool _ignoreCase;

        public TextContainsMatch(string value, bool ignoreCase) {

            if ( value.StartsWith(Resources.NotPattern, StringComparison.OrdinalIgnoreCase) ) {
                _not = true;
                value = value.Remove(0, Resources.NotPattern.Length);
            }

            if ( value.StartsWith(Resources.RegExPattern, StringComparison.OrdinalIgnoreCase) ) {
                _matchUsesRegex = true;
                value = value.Remove(0, Resources.RegExPattern.Length);

                if ( ignoreCase ) {
                    _matchRegexValue = new Regex(value, RegexOptions.IgnoreCase);
                }
                else {
                    _matchRegexValue = new Regex(value);
                }

            }
            else {
                _matchStringValue = value;
            }

            _ignoreCase = ignoreCase;

        }

        public bool IsMatch(string value) {

            var comparison = _ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            var match = _matchUsesRegex ? _matchRegexValue.IsMatch(value) : (value.IndexOf(_matchStringValue, 0, comparison) > -1);
            match = match ^ _not;

            return match;

        }

    }

}
