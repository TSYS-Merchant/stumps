namespace Stumps
{
    using System;
    using System.Security.Cryptography;
    using Stumps.Rules;

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    public static class FluentStumpExtensions
    {
        /// <summary>
        ///     Specifies the amount of time the server delays before responding with the Stump.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="delayMilliseconds">The amount of time, in milliseconds, the response is delayed.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump DelayedBy(this Stump stump, int delayMilliseconds)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.ResponseDelay = delayMilliseconds;
            return stump;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.Stump"/> will drop the connection immediately.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump DropsConnection(this Stump stump)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.TerminateConnection = true;

            stump.Responds();

            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="buffer">The array of bytes for the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBody(this Stump stump, byte[] buffer)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            if (buffer != null)
            {
                stump.AddRule(new BodyMatchRule(buffer.Length, CreateMd5Hash(buffer)));
            }

            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to contain the specified text in the body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="text">The text that must be contained within the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBodyContaining(this Stump stump, string text)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            var textArray = new[] { text };

            var stumpResponse = stump.MatchingBodyContaining(textArray);
            return stumpResponse;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to contain the specified text in the body.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="text">The text that must be contained within the body.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingBodyContaining(this Stump stump, string[] text)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.AddRule(new BodyContentRule(text));
            return stump;
        }
        
        /// <summary>
        ///     Requires the incoming HTTP request to match the specified header.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="headerName">The name of the header to match.</param>
        /// <param name="headerValue">The value of the header to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingHeader(this Stump stump, string headerName, string headerValue)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.AddRule(new HeaderRule(headerName, headerValue));
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified HTTP method.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="httpMethod">The HTTP method to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingMethod(this Stump stump, string httpMethod)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.AddRule(new HttpMethodRule(httpMethod));
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified <see cref="T:Stumps.IStumpRule"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="rule">The <see cref="T:Stumps.IStumpRule"/> required to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump MatchingRule(this Stump stump, IStumpRule rule)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.AddRule(rule);
            return stump;
        }

        /// <summary>
        ///     Requires the incoming HTTP request to match the specified URL.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="url">The URL to match.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "The value is for a text match against a URL and not a URI.")]
        public static Stump MatchingUrl(this Stump stump, string url)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.AddRule(new UrlRule(url));
            return stump;
        }

        /// <summary>
        ///     Asserts that the <see cref="T:Stumps.Stump"/> will respond with a <see cref="T:Stumps.BasicHttpResponse"/>.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> intercepting incoming HTTP requests.</param>
        /// <returns>A <see cref="T:Stumps.BasicHttpResponse"/> created for the <paramref name="stump"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static BasicHttpResponse Responds(this Stump stump)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            stump.ResponseFactory = stump.ResponseFactory ?? new StumpResponseFactory();

            var response = new BasicHttpResponse();
            stump.ResponseFactory.Add(response);

            return response;
        }

        /// <summary>
        ///     Asserts that the <see cref="StumpResponseFactory"/> will respond with a <see cref="BasicHttpResponse"/>.
        /// </summary>
        /// <param name="responseFactory">The <see cref="StumpResponseFactory"/> to which the new <see cref="BasicHttpResponse"/> is added.</param>
        /// <returns>A <see cref="BasicHttpResponse"/> created for the <paramref name="responseFactory"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="responseFactory"/> is <c>null</c>.</exception>
        public static BasicHttpResponse Responds(this StumpResponseFactory responseFactory)
        {
            responseFactory = responseFactory ?? throw new ArgumentNullException(nameof(responseFactory));

            var response = new BasicHttpResponse();
            responseFactory.Add(response);

            return response;
        }

        /// <summary>
        /// Respondswithes the multiple.
        /// </summary>
        /// <param name="stump">The <see cref="Stump"/> intercepting incoming HTTP requests.</param>
        /// <param name="behavior">The behavior of the <see cref="StumpResponseFactory.CreateResponse(IStumpsHttpRequest)"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        /// <returns>The calling <see cref="T:Stumps.Stump"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        public static Stump ReturnsMultipleResponses(this Stump stump, ResponseFactoryBehavior behavior)
        {
            stump = stump ?? throw new ArgumentNullException(nameof(stump));

            if (stump.ResponseFactory == null || stump.ResponseFactory.Behavior != behavior)
            {
                stump.ResponseFactory = new StumpResponseFactory(behavior);
            }

            return stump;
        }

        /// <summary>
        ///     Creates an MD5 hash for the specified bytes.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>The MD5 hash of the specified <paramref name="buffer"/>.</returns>
        private static string CreateMd5Hash(byte[] buffer)
        {
            string result;

            using (var hash = MD5.Create())
            {
                var bytes = hash.ComputeHash(buffer);
                result = bytes.ToHexString();
            }

            return result;
        }
    }
}
