namespace Stumps.Http
{

    using System;
    using System.Globalization;
    using System.Net;
    using NUnit.Framework;

    [TestFixture]
    public class StumpsHttpRequestTests
    {

        [Test]
        public void StumpsHttpRequest_PopulatesCorrectly()
        {

            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);
            mockHandler.ContentType = "bobs.type";

            var startingEventCount = 0;

            using (var server = HttpHelper.CreateHttpServer(mockHandler))
            {

                server.RequestStarting += (o, i) =>
                {
                    var request = i.Context.Request;

                    startingEventCount++;
                    Assert.Greater(request.Headers.Count, 0);
                    Assert.AreEqual("GET", request.HttpMethod);
                    Assert.AreEqual(server.Port, request.LocalEndPoint.Port);
                    Assert.AreEqual("1.1", request.ProtocolVersion);
                    Assert.AreEqual("/", request.RawUrl);

                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var webRequest = WebRequest.CreateHttp(uri);
                webRequest.ContentType = "Bobs.Content";
                webRequest.Referer = "http://stumps-project.com/";
                webRequest.UserAgent = "StumpsTestAgent";

                using (var response = (HttpWebResponse)webRequest.GetResponse())
                {
                    Assert.IsNotNull(response);
                }

            }

            Assert.AreEqual(startingEventCount, 1);

        }

    }

}