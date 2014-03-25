namespace Stumps
{

    using System;

    /// <summary>
    ///     A class that determins if a block of text contains a specified value.
    /// </summary>
    internal class TextContainsMatch : TextMatch
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.TextContainsMatch"/> class.
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="ignoreCase">If set to <c>true</c>, capitalization is ignored.</param>
        public TextContainsMatch(string value, bool ignoreCase) : base(value, ignoreCase)
        {
        }

        /// <summary>
        ///     Determines whether the specified value matches the text.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns><c>true</c> if the text matches; otherwise, <c>false</c>.</returns>
        public override bool IsMatch(string value)
        {

            var match = this.UseRegexEvaluation
                            ? this.RegexValue.IsMatch(value)
                            : (value.IndexOf(this.StringValue, 0, this.ComparisonMethod) > -1);
            match = match ^ this.InverseEvaluation;

            return match;

        }

    }

}