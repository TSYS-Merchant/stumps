namespace Stumps.Http
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
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

            var requestFinishedEvent = new AutoResetEvent(false);

            using (var server = HttpHelper.CreateServer(mockHandler))
            {
                server.RequestFinished += (o, i) =>
                {
                    var response = i.Context.Response;

                    Assert.IsNotNull(response.Headers);
                    Assert.Greater(response.Headers.Count, 0);
                    Assert.AreEqual(response.Headers["X-Stumps"], "V1");
                    requestFinishedEvent.Set();
                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.Create(uri);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.IsNotNull(response);
                }
            }

            var requestFinished = requestFinishedEvent.WaitOne(1000);
            Assert.IsTrue(requestFinished);
        }
    }
}
