namespace Stumps.Http
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class that determines if the HTTP header shouold be ignored and not transferred to the response.
    /// </summary>
    internal static class IgnoredHeaders
    {

        private static readonly HashSet<string> KnownHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "content-length",
            "content-type",
            "transfer-encoding",
            "keep-alive"
        };

        /// <summary>
        /// Determines whether the specified header name is ignored.
        /// </summary>
        /// <param name="headerName">THe name of the header.</param>
        /// <returns><c>true</c> if the header is ignored; otherwise, <c>false</c>.</returns>
        public static bool IsIgnored(string headerName)
        {
            return KnownHeaders.Contains(headerName);
        }

    }

}
