namespace Stumps
{

    using System;
    using System.IO;
    using System.Web;

    /// <summary>
    ///     A class that provides a set of Fluent extension methods to <see cref="T:Stumps.BasicHttpResponse"/> objects.
    /// </summary>
    public static class FluentBasicHttpResponseExtensions
    {

        /// <summary>
        ///     Specifies the body returned as part of the HTTP response.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse" /> that returns in response to an HTTP request.</param>
        /// <param name="buffer">The byte array to return as the body of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithBody(this BasicHttpResponse response, byte[] buffer)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.ClearBody();
            response.AppendToBody(buffer);
            return response;

        }

        /// <summary>
        /// Specifies the body returned as part of the HTTP response.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse" /> that returns in response to an HTTP request.</param>
        /// <param name="body">The value returned as body of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithBody(this BasicHttpResponse response, string body)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.ClearBody();
            response.AppendToBody(body);
            return response;

        }

        /// <summary>
        ///     Specifies the body returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="path">The path to the file that contains the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithFile(this BasicHttpResponse response, string path)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var buffer = File.ReadAllBytes(path);

            response.ClearBody();
            response.AppendToBody(buffer);

            var mimeType = MimeMapping.GetMimeMapping(path);
            response.Headers["Content-Type"] = mimeType;

            return response;

        }

        /// <summary>
        ///     Specifies a header returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="headerName">The name of the header.</param>
        /// <param name="headerValue">The value of the header.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithHeader(this BasicHttpResponse response, string headerName, string headerValue)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }
            
            response.Headers[headerName] = headerValue;
            return response;

        }

        /// <summary>
        ///     Specifies the status code returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="statusCode">The status code to return.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithStatusCode(this BasicHttpResponse response, int statusCode)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.StatusCode = statusCode;
            return response;

        }

        /// <summary>
        ///     Specifies the description of the status code returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="statusDescription">The description of the status code.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithStatusDescription(this BasicHttpResponse response, string statusDescription)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.StatusDescription = statusDescription;
            return response;

        }

        /// <summary>
        ///     Specifies the redirect address returned as part of the HTTP response. 
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.BasicHttpResponse"/> that returns in response to an HTTP request.</param>
        /// <param name="redirectAddress">The redirect address returned as part of the HTTP response.</param>
        /// <returns>The calling <see cref="T:Stumps.BasicHttpResponse"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        public static BasicHttpResponse WithRedirectAddress(this BasicHttpResponse response, string redirectAddress)
        {

            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            response.RedirectAddress = redirectAddress;
            return response;

        }

    }

}
