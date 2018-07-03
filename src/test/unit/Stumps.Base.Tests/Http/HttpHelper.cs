namespace Stumps.Http
{
    using NSubstitute;

    internal static class HttpHelper
    {
        public static HttpServer CreateServer()
        {
            var handler = Substitute.For<IHttpHandler>();
            return HttpHelper.CreateServer(handler);
        }

        public static HttpServer CreateServer(IHttpHandler handler)
        {
            var openPort = NetworkInformation.FindRandomOpenPort();

            var server = new HttpServer(openPort, handler);

            return server;
        }
    }
}
