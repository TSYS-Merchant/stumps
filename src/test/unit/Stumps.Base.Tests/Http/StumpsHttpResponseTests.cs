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

            var finishingEventCount = 0;

            using (var server = HttpHelper.CreateServer(mockHandler))
            {

                server.RequestFinished += (o, i) =>
                {
                    var response = i.Context.Response;

                    finishingEventCount++;
                    Assert.IsNotNull(response.Headers);
                    Assert.Greater(response.Headers.Count, 0);
                    Assert.AreEqual(response.Headers["X-Stumps"], "V1");

                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.Create(uri);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.IsNotNull(response);
                }

            }

            Assert.AreEqual(finishingEventCount, 1);

        }

    }

}
