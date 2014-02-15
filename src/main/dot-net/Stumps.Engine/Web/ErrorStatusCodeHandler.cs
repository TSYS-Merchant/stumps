namespace Stumps.Web
{

    using System;
    using System.Linq;
    using Nancy;
    using Nancy.ErrorHandling;
    using Nancy.Responses.Negotiation;
    using Stumps.Web.Responses;

    public sealed class ErrorStatusCodeHandler : IStatusCodeHandler
    {

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var clientWantsHtml = ShouldReturnFriendlyErrorPage(context);

            if (!clientWantsHtml && context.Response is NotFoundResponse)
            {
                context.Response = ErrorJsonResponse.FromMessage(Resources.ErrorResourceNotFound);
            }
            else
            {
                context.Response = new ErrorHtmlResponse(statusCode);
            }

        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, Nancy.NancyContext context)
        {
            return statusCode == HttpStatusCode.NotFound || statusCode == HttpStatusCode.InternalServerError;
        }

        private bool ShouldReturnFriendlyErrorPage(NancyContext context)
        {

            var enumerable = context.Request.Headers.Accept;

            var ranges = enumerable.OrderByDescending(o => o.Item2).Select(o => MediaRange.FromString(o.Item1)).ToList();

            foreach (var item in ranges)
            {

                if (item.Matches(Resources.ContentTypeApplicationJson) || item.Matches(Resources.ContentTypeTextJson))
                {
                    return false;
                }

                if (item.Matches(Resources.ContentTypeHtml))
                {
                    return true;
                }

            }

            return true;

        }

    }

}