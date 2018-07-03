namespace Stumps
{
    using System.Collections.Generic;

    /// <summary>
    ///     Sanitizes header values
    /// </summary>
    internal static class HttpHeaderSanitization
    {
        private static readonly HashSet<char> InvalidHeaderNameCharacters = new HashSet<char>
        {
            '"', '\'', '(', ')', ',', '/', ':', ';', '<', '=', '>', '?', '@', '[', '\\', ']', '{', '}'
        };

        /// <summary>
        ///     Sanitizes the name and value of a header.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <param name="value">The header value.</param>
        /// <returns>
        ///     <c>true</c> if the value is safe to be written; otherwise, <c>false</c>.
        /// </returns>
        public static bool SanitizeHeader(ref string name, ref string value)
        {
            var response = SanitizeHeaderPart(ref name, false);
            response = response & SanitizeHeaderPart(ref value, true);

            return response;
        }

        /// <summary>
        /// Sanitizes the header part.
        /// </summary>
        /// <param name="value">The value to sanitize.</param>
        /// <param name="isHeaderValue">if set to <c>true</c>, the value is a header value and not a header name.</param>
        /// <returns>
        ///     <c>true</c> if the value is safe to be written; otherwise, <c>false</c>.
        /// </returns>
        public static bool SanitizeHeaderPart(ref string value, bool isHeaderValue)
        {
            // First-Pass sanity check
            var response = EmptyCheck(ref value, isHeaderValue);

            if (!response || value.Length == 0)
            {
                return response;
            }

            response = isHeaderValue ?
                SanitizeHeaderValue(ref value) :
                SanitizeHeaderName(ref value);

            return response;
        }

        /// <summary>
        ///     Checks the value to determine if it is empty, ad if it is allowed.
        /// </summary>
        /// <param name="value">The value. to check.</param>
        /// <param name="isHeaderValue">if set to <c>true</c>, the value is a header value and not a header name.</param>
        /// <returns>
        ///     <c>true</c> if the value is safe to be written; otherwise, <c>false</c>.
        /// </returns>
        private static bool EmptyCheck(ref string value, bool isHeaderValue)
        {
            if (value == null || value.Length == 0)
            {
                if (!isHeaderValue)
                {
                    return false;
                }

                // Empty value is OK
                value = string.Empty;
            }

            return true;
        }

        /// <summary>
        ///     Sanitizes the header name.
        /// </summary>
        /// <param name="value">The value for the header name.</param>
        /// <returns></returns>
        private static bool SanitizeHeaderName(ref string value)
        {
            var newValue = string.Empty;
            var valueSanitized = false;

            // Strip out bad characters
            foreach (var c in value)
            {
                if (InvalidHeaderNameCharacters.Contains(c) || c < 0x20 || c > 0x7e)
                {
                    valueSanitized = true;
                }
                else
                {
                    newValue += c;
                }
            }

            if (!valueSanitized)
            {
                return true;
            }

            var response = EmptyCheck(ref newValue, false);
            value = newValue;

            return response;
        }

        /// <summary>
        ///     Sanitizes the header value.
        /// </summary>
        /// <param name="value">The value for the header.</param>
        /// <returns></returns>
        /// <remarks>
        ///     This may cause a break with multi-line header values.
        /// </remarks>
        private static bool SanitizeHeaderValue(ref string value)
        {
            var newValue = string.Empty;
            var valueSanitized = false;

            // WARNING this does break the multi-line headers
            foreach (var c in value)
            {
                if (c == ' ' || c == '\t' || (c >= 0x20 && c <= 0x7e))
                {
                    newValue += c;
                }
                else
                {
                    valueSanitized = true;
                }
            }

            if (!valueSanitized)
            {
                return true;
            }

            var response = EmptyCheck(ref newValue, false);
            value = newValue;

            return response;
        }
    }
}
