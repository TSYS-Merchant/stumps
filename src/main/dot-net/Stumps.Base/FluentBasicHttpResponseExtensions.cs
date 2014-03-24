namespace Stumps
{

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to <see cref="T:Stumps.BasicHttpResponse"/> objects.
    /// </summary>
    public static class FluentBasicHttpResponseExtensions
    {

        /// <summary>
        ///     Specifies the body returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="buffer">The byte array to return as the body of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithBody(this BasicHttpResponse response, byte[] buffer)
        {
            response.ClearBody();
            response.AppendToBody(buffer);
            return response;
        }

        /// <summary>
        ///     Specifies the body returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="body">The value returned as body of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithBody(this BasicHttpResponse response, string body)
        {
            response.ClearBody();
            response.AppendToBody(body);
            return response;
        }

        /// <summary>
        ///     Specifies a header returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="headerName">The name of the header.</param>
        /// <param name="headerValue">The value of the header.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithHeader(this BasicHttpResponse response, string headerName, string headerValue)
        {
            response.Headers[headerName] = headerValue;
            return response;
        }

        /// <summary>
        ///     Specifies the status code returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="statusCode">The status code to return.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithStatusCode(this BasicHttpResponse response, int statusCode)
        {
            response.StatusCode = statusCode;
            return response;
        }

        /// <summary>
        ///     Specifies the description of the status code returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="statusDescription">The description of the status code.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithStatusDescription(this BasicHttpResponse response, string statusDescription)
        {
            response.StatusDescription = statusDescription;
            return response;
        }

        /// <summary>
        ///     Specifies the redirect address returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="redirectAddress">The redirect address returned as part of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        public static BasicHttpResponse WithRedirectAddress(this BasicHttpResponse response, string redirectAddress)
        {
            response.RedirectAddress = redirectAddress;
            return response;
        }

    }

}
