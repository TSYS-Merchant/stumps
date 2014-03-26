namespace Stumps
{

    using System;
    using Stumps.Rules;

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    public static class FluentStumpExtensions
    {

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="buffer">The array of bytes for the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBody(this Stump stump, byte[] buffer)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            stump.AddRule(new BodyMatchRule(buffer));
            return stump;

        }

        /// <summary>
        ///     Requires the incoming HTTP request to contain the specified text in the body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="text">The text that must be contained within the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBodyContaining(this Stump stump, string text)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

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
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBodyContaining(this Stump stump, string[] text)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

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
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingHeader(this Stump stump, string headerName, string headerValue)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            stump.AddRule(new HeaderRule(headerName, headerValue));
            return stump;

        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified HTTP method.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="httpMethod">The HTTP method to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingMethod(this Stump stump, string httpMethod)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            stump.AddRule(new HttpMethodRule(httpMethod));
            return stump;

        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified <see cref="T:Stumps.IStumpRule"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="rule">The <see cref="T:Stumps.IStumpRule"/> required to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingRule(this Stump stump, IStumpRule rule)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            stump.AddRule(rule);
            return stump;

        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified URL.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <param name="url">The URL to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The value is for a text match against a URL and not a URI.")]
        public static Stump MatchingUrl(this Stump stump, string url)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            stump.AddRule(new UrlRule(url));
            return stump;

        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.Stump"/> will respond with a <see cref="T:Stumps.BasicHttpResponse"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incomming HTTP requests.</param>
        /// <returns>A <see cref="T:Stumps.BasicHttpResponse"/> created for the <paramref name="stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static BasicHttpResponse Responds(this Stump stump)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            var response = new BasicHttpResponse();
            stump.Response = response;
            return response;

        }

    }

}
