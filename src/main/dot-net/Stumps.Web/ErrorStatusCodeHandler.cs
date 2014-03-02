namespace Stumps.Web
{

    using System;
    using System.Linq;
    using Nancy;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Stumps.Web.Responses;

    /// <summary>
    ///     A class that provides an implementation of the Nancy status code handler.
    /// </summary>
    public sealed class ErrorStatusCodeHandler : IStatusCodeHandler
    {

        /// <summary>
        /// Handle the error code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code</param>
        /// <param name="context">Current context of the request.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var clientWantsHtml = ShouldReturnFriendlyErrorPage(context);

            if (!clientWantsHtml && context.Response is NotFoundResponse)
            {
                context.Response = ErrorJsonResponse.FromMessage(WebResources.ErrorResourceNotFound);
            }
            else
            {
                context.Response = new ErrorHtmlResponse(statusCode);
            }

        }

        /// <summary>
        /// Check if the error handler can handle errors of the provided status code.
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="T:Nancy.NancyContext" /> instance of the current request.</param>
        /// <returns>
        /// True if handled, false otherwise
        /// </returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode, Nancy.NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.InternalServerError;
        }

        /// <summary>
        ///     Determines if a friendly error page should be returned for a <see cref="T:Nancy.NancyContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="T:Nancy.NancyContext"/>.</param>
        /// <returns><c>true</c> if an HTML page should be returned; otherwise, <c>false</c>.</returns>
        private bool ShouldReturnFriendlyErrorPage(NancyContext context)
        {

            var enumerable = context.Request.Headers.Accept;

            var ranges = enumerable.OrderByDescending(o => o.Item2).Select(o => MediaRange.FromString(o.Item1)).ToList();

            foreach (var item in ranges)
            {

                if (item.Matches(WebResources.ContentTypeApplicationJson) || item.Matches(WebResources.ContentTypeTextJson))
                {
                    return false;
                }

                if (item.Matches(WebResources.ContentTypeHtml))
                {
                    return true;
                }

            }

            return true;

        }

    }

}