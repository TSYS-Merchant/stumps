namespace Stumps.Http
{

    using System;
    using System.Globalization;
    using System.Net;
    using NUnit.Framework;

    [TestFixture]
    public class StumpsHttpResponseTests
    {

        [Test]
        public void StumpsHttpResponse_PopulatesCorrectly()
        {

            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);
            mockHandler.AddHeader("X-Stumps", "V1");

            var startingEventCount = 0;
            var finishingEventCount = 0;

            using (var server = HttpHelper.CreateHttpServer(mockHandler))
            {

                server.RequestFinishing += (o, i) =>
                {
                    var response = i.Context.Response;

                    finishingEventCount++;
                    Assert.AreEqual(mockHandler.ContentType, response.ContentType);
                    Assert.IsNotNull(response.Headers);
                    Assert.Greater(response.Headers.Count, 0);
                    Assert.AreEqual(response.Headers["X-Stumps"], "V1");

                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.CreateHttp(uri);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.IsNotNull(response);
                }

            }

            Assert.AreEqual(finishingEventCount, 1);

        }

    }

}