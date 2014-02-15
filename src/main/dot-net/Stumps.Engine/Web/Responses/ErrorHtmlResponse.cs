namespace Stumps.Web.Responses
{

    using Nancy;
    using Nancy.Responses;
    using Stumps.Utility;

    public class ErrorHtmlResponse : HtmlResponse
    {

        public ErrorHtmlResponse(HttpStatusCode statusCode)
        {

            this.StatusCode = statusCode;
            this.ContentType = Resources.ContentTypeHtmlUtf8;

            this.Contents = stream =>
            {

                var page = string.Empty;

                switch (this.StatusCode)
                {

                    case HttpStatusCode.NotFound:
                        page = Resources.PageNotFound;
                        break;

                    case HttpStatusCode.InternalServerError:
                        page = Resources.PageInternalServerError;
                        break;

                }

                StreamUtility.WriteUtf8StringToStream(page, stream);

            };

        }

    }

}