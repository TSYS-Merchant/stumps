namespace Stumps
{

    using Stumps.Rules;

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    public static class FulentStumpExtensions
    {

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="buffer">The array of bytes for the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingBody(this Stump stump, byte[] buffer)
        {
            stump.AddRule(new BodyMatchRule(buffer));
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to contain the specified text in the body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="text">The text that must be contained within the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingBodyContaining(this Stump stump, string text)
        {
            var textArray = new[] { text };

            var stumpResponse = stump.MatchingBodyContaining(textArray);
            return stumpResponse;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to contain the specified text in the body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="text">The text that must be contained within the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingBodyContaining(this Stump stump, string[] text)
        {
            stump.AddRule(new BodyContentRule(text));
            return stump;
        }
        
        /// <summary>
        ///     Requires the incoming HTTP request to match the specified header.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="headerName">The name of the header to match.</param>
        /// <param name="headerValue">The value of the header to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingHeader(this Stump stump, string headerName, string headerValue)
        {
            stump.AddRule(new HeaderRule(headerName, headerValue));
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified HTTP method.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="httpMethod">The HTTP method to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingMethod(this Stump stump, string httpMethod)
        {
            stump.AddRule(new HttpMethodRule(httpMethod));
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified <see cref="T:Stumps.IStumpRule"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="rule">The <see cref="T:Stumps.IStumpRule"/> required to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingRule(this Stump stump, IStumpRule rule)
        {
            stump.AddRule(rule);
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified URL.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="url">The URL to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        public static Stump MatchingUrl(this Stump stump, string url)
        {
            stump.AddRule(new UrlRule(url));
            return stump;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.Stump"/> will respond with a <see cref="T:Stumps.BasicHttpResponse"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <returns>A <see cref="T:Stumps.BasicHttpResponse"/> created for the <paramref name="stump"/>.</returns>
        public static BasicHttpResponse Responds(this Stump stump)
        {
            var response = new BasicHttpResponse();
            stump.Response = response;
            return response;
        }

    }

}
