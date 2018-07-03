namespace Stumps
{
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     A class that provides text matching for various rules.
    /// </summary>
    internal class TextMatch
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.TextMatch"/> class.
        /// </summary>
        /// <param name="value">The value used to match.</param>
        /// <param name="ignoreCase">If set to <c>true</c>, capitalization is ignored.</param>
        public TextMatch(string value, bool ignoreCase)
        {
            if (value.StartsWith(BaseResources.NotPattern, StringComparison.OrdinalIgnoreCase))
            {
                this.InverseEvaluation = true;
                value = value.Remove(0, BaseResources.NotPattern.Length);
            }

            if (value.StartsWith(BaseResources.RegExPattern, StringComparison.OrdinalIgnoreCase))
            {
                this.UseRegexEvaluation = true;
                value = value.Remove(0, BaseResources.RegExPattern.Length);

                this.RegexValue = ignoreCase ? new Regex(value, RegexOptions.IgnoreCase) : new Regex(value);
            }
            else
            {
                this.StringValue = value;
            }

            this.ComparisonMethod = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        }

        /// <summary>
        ///     Gets the comparison method to use when evaluating strings.
        /// </summary>
        /// <value>
        ///   The comparison method to use when evaluating strings.
        /// </value>
        protected StringComparison ComparisonMethod
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether to inverse the result of the match.
        /// </summary>
        /// <value>
        ///   <c>true</c> if to inverse the result of the match; otherwise, <c>false</c>.
        /// </value>
        protected bool InverseEvaluation
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the regular expression required to match a specified value.
        /// </summary>
        /// <value>
        ///     The regular expression required to match a specified value.
        /// </value>
        protected Regex RegexValue
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the string required to match a specified value.
        /// </summary>
        /// <value>
        ///     The string required to match a specified value.
        /// </value>
        protected string StringValue
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets a value indicating whether to match using regular expressions.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the matching should use regular expressions; otherwise, <c>false</c>.
        /// </value>
        protected bool UseRegexEvaluation
        {
            get;
            private set;
        }

        /// <summary>
        ///     Determines whether the specified value matches the text.
        /// </summary>
        /// <param name="value">The value to evaluate.</param>
        /// <returns><c>true</c> if the text matches; otherwise, <c>false</c>.</returns>
        public virtual bool IsMatch(string value)
        {
            var match = this.UseRegexEvaluation ? this.RegexValue.IsMatch(value) : this.StringValue.Equals(value, this.ComparisonMethod);
            match = match ^ this.InverseEvaluation;

            return match;
        }
    }
}