namespace Stumps.Http
{

    using NSubstitute;

    internal static class HttpHelper
    {

        public static HttpServer CreateHttpServer()
        {

            var handler = Substitute.For<IHttpHandler>();
            return HttpHelper.CreateHttpServer(handler);

        }

        public static HttpServer CreateHttpServer(IHttpHandler handler)
        {

            var openPort = NetworkInformation.FindRandomOpenPort();

            var server = new HttpServer(openPort, handler);

            return server;

        }

    }

}