namespace Stumps.Http
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Threading;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class HttpServerTests
    {
        [Test]
        public void Constructor_WithInvalidHandler_ThrowsException()
        {
            IHttpHandler handler = null;

            Assert.That(
                () => new HttpServer(8080, handler),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("handler"));
        }

        [Test]
        public void Constructor_WithInvalidPort_ThrowsException()
        {
            int exceedsMaximumPort = IPEndPoint.MaxPort + 1;
            int exceedsMinimumPort = IPEndPoint.MinPort - 1;

            IHttpHandler handler = Substitute.For<IHttpHandler>();

            Assert.That(
                () => new HttpServer(exceedsMaximumPort, handler),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));

            Assert.That(
                () => new HttpServer(exceedsMinimumPort, handler),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));
        }

        [Test]
        public void HttpServer_StartStop_Success()
        {
            Assert.DoesNotThrow(
                () =>
                {
                    using (var server = HttpHelper.CreateServer())
                    {
                        server.StartListening();
                        server.StopListening();
                    }
                });
        }

        [Test]
        public void HttpServer_StartStop_UpdatesProperty()
        {
            using (var server = HttpHelper.CreateServer())
            {
                Assert.IsFalse(server.Started);

                server.StartListening();

                Assert.IsTrue(server.Started);

                server.StopListening();

                Assert.IsFalse(server.Started);
            }
        }

        [Test]
        public void ProcessAsyncRequest_WithValidRequest_RaisesServerEvents()
        {
            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);

            var requestReceivedEvent = new AutoResetEvent(false);
            var requestFinishedEvent = new AutoResetEvent(false);

            using (var server = HttpHelper.CreateServer(mockHandler))
            {
                server.RequestReceived += (o, i) =>
                {
                    Assert.IsNotNull(o);
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.Context);
                    Assert.IsNotNull(i.Context.Request);
                    Assert.IsNotNull(i.Context.Response);
                    requestReceivedEvent.Set();
                };

                server.RequestFinished += (o, i) =>
                {
                    Assert.IsNotNull(o);
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.Context);
                    Assert.IsNotNull(i.Context.Request);
                    Assert.IsNotNull(i.Context.Response);
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

            var requestReceived = requestReceivedEvent.WaitOne(1000);
            var requestFinished = requestFinishedEvent.WaitOne(1000);

            Assert.IsTrue(requestReceived, "Request received.");
            Assert.IsTrue(requestFinished, "Request finished.");
        }

        [Test]
        public void ProcessAsyncRequest_WithValidRequest_ReturnsResponseFromHandler()
        {
            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);

            using (var server = HttpHelper.CreateServer(mockHandler))
            {
                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.Create(uri);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
                    Assert.AreEqual(mockHandler.StatusDescription, response.StatusDescription);

                    string body;

                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        body = sr.ReadToEnd();
                    }

                    Assert.AreEqual(TestData.SampleTextResponse, body);
                }
            }
        }

        [Test]
        public void StartListening_CalledTwice_Accepted()
        {
            Assert.DoesNotThrow(
                () =>
                {
                    using (var server = HttpHelper.CreateServer())
                    {
                        server.StartListening();
                        server.StartListening();
                        server.StopListening();
                    }
                });
        }

        [Test]
        public void StopListening_CalledTwice_Accepted()
        {
            Assert.DoesNotThrow(
                () =>
                {
                    using (var server = HttpHelper.CreateServer())
                    {
                        server.StartListening();
                        server.StopListening();
                        server.StopListening();
                    }
                });
        }
    }
}
