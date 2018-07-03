namespace Stumps.Http
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class StumpsHttpRequestTests
    {
        [Test]
        public void StumpsHttpRequest_PopulatesCorrectly()
        {
            var mockHandler = new MockHandler
            {
                StatusCode = 202,
                StatusDescription = "Bob"
            };

            mockHandler.UpdateBody(TestData.SampleTextResponse);
            mockHandler.ContentType = "bobs.type";

            var requestReceivedEvent = new AutoResetEvent(false);

            using (var server = HttpHelper.CreateServer(mockHandler))
            {
                server.RequestReceived += (o, i) =>
                {
                    var request = i.Context.Request;

                    Assert.Greater(request.Headers.Count, 0);
                    Assert.AreEqual("POST", request.HttpMethod);
                    Assert.AreEqual(server.Port, request.LocalEndPoint.Port);
                    Assert.AreEqual("1.1", request.ProtocolVersion);
                    Assert.AreEqual("/", request.RawUrl);
                    Assert.AreEqual(3, request.BodyLength);
                    CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, request.GetBody());
                    requestReceivedEvent.Set();
                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var webRequest = WebRequest.Create(uri) as System.Net.HttpWebRequest;
                webRequest.ContentType = "Bobs.Content";
                webRequest.Referer = "http://stumps-project.com/";
                webRequest.UserAgent = "StumpsTestAgent";
                webRequest.Method = "POST";
                webRequest.ContentLength = 3;
                var stream = webRequest.GetRequestStream();
                stream.Write(new byte[] { 1, 2, 3 }, 0, 3);

                stream.Close();
                using (var response = (HttpWebResponse)webRequest.GetResponse())
                {
                    Assert.IsNotNull(response);
                }
            }

            var requestReceived = requestReceivedEvent.WaitOne(1000);
            Assert.IsTrue(requestReceived);
        }
    }
}
