namespace Stumps.Http
{

    using NSubstitute;
    using Stumps.Logging;
    using Stumps.Utility;

    internal static class HttpHelper
    {

        public static HttpServer CreateHttpServer()
        {

            var handler = Substitute.For<IHttpHandler>();
            return HttpHelper.CreateHttpServer(handler);

        }

        public static HttpServer CreateHttpServer(IHttpHandler handler)
        {

            var openPort = NetworkUtility.FindRandomOpenPort();

            var logger = Substitute.For<ILogger>();

            var server = new HttpServer(openPort, handler, logger);

            return server;

        }

    }

}