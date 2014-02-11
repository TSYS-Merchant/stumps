﻿namespace Stumps.Http {

    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Logging;

    [TestFixture]
    public class HttpServerTests {

        [Test]
        public void Constructor_WithInvalidHandler_ThrowsException() {

            ILogger logger = Substitute.For<ILogger>();
            IHttpHandler handler = null;

            Assert.That(
                () => new HttpServer(8080, handler, logger),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("handler")
            );

        }

        [Test]
        public void Constructor_WithInvalidLogger_ThrowsException() {

            ILogger logger = null;
            IHttpHandler handler = Substitute.For<IHttpHandler>();

            Assert.That(
                () => new HttpServer(8080, handler, logger),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("logger")
            );

        }

        [Test]
        public void Constructor_WithInvalidPort_ThrowsException() {

            int exceedsMaximumPort = IPEndPoint.MaxPort + 1;
            int exceedsMinimumPort = IPEndPoint.MinPort - 1;

            ILogger logger = Substitute.For<ILogger>();
            IHttpHandler handler = Substitute.For<IHttpHandler>();

            Assert.That(
                () => new HttpServer(exceedsMaximumPort, handler, logger),
                Throws.Exception
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName")
                    .EqualTo("port")
            );

            Assert.That(
                () => new HttpServer(exceedsMinimumPort, handler, logger),
                Throws.Exception
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName")
                    .EqualTo("port")
            );

        }

        [Test]
        public void HttpServer_StartStop_Success() {

            Assert.DoesNotThrow(() => {
                using ( var server = HttpHelper.CreateHttpServer() ) {

                    server.StartListening();
                    server.StopListening();

                }
            });

        }

        [Test]
        public void HttpServer_StartStop_UpdatesProperty() {

            using ( var server = HttpHelper.CreateHttpServer() ) {

                Assert.IsFalse(server.Started);

                server.StartListening();

                Assert.IsTrue(server.Started);

                server.StopListening();

                Assert.IsFalse(server.Started);

            }

        }

        [Test]
        public void ProcessAsyncRequest_WithValidRequest_ReturnsResponseFromHandler() {

            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);

            using ( var server = HttpHelper.CreateHttpServer(mockHandler) ) {

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.CreateHttp(uri);

                using ( var response = (HttpWebResponse) request.GetResponse() ) {

                    Assert.AreEqual(HttpStatusCode.Accepted, response.StatusCode);
                    Assert.AreEqual(mockHandler.StatusDescription, response.StatusDescription);

                    string body;

                    using ( var sr = new StreamReader(response.GetResponseStream()) ) {
                        body = sr.ReadToEnd();
                    }

                    Assert.AreEqual(TestData.SampleTextResponse, body);

                }

            }

        }

        [Test]
        public void ProcessAsyncRequest_WithValidRequest_RaisesServerEvents() {

            var mockHandler = new MockHandler();
            mockHandler.StatusCode = 202;
            mockHandler.StatusDescription = "Bob";
            mockHandler.UpdateBody(TestData.SampleTextResponse);

            var startingEventCount = 0;
            var finishingEventCount = 0;

            using ( var server = HttpHelper.CreateHttpServer(mockHandler) ) {

                server.RequestStarting += (o, i) => {
                    startingEventCount++;
                    Assert.IsNotNull(o);
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.Context);
                    Assert.IsNotNull(i.Context.Request);
                    Assert.IsNotNull(i.Context.Response);
                };

                server.RequestFinishing += (o, i) => {
                    finishingEventCount++;
                    Assert.IsNotNull(o);
                    Assert.IsNotNull(i);
                    Assert.IsNotNull(i.Context);
                    Assert.IsNotNull(i.Context.Request);
                    Assert.IsNotNull(i.Context.Response);
                };

                server.StartListening();

                var uri = new Uri("http://localhost:" + server.Port.ToString(CultureInfo.InvariantCulture) + "/");

                var request = WebRequest.CreateHttp(uri);

                using ( var response = (HttpWebResponse) request.GetResponse() ) {
                    Assert.IsNotNull(response);
                }

            }

            Assert.AreEqual(startingEventCount, 1);
            Assert.AreEqual(finishingEventCount, 1);

        }

        [Test]
        public void StartListening_CalledTwice_Accepted() {

            Assert.DoesNotThrow(() => {
                using ( var server = HttpHelper.CreateHttpServer() ) {

                    server.StartListening();
                    server.StartListening();
                    server.StopListening();

                }
            });

        }

        [Test]
        public void StopListening_CalledTwice_Accepted() {

            Assert.DoesNotThrow(() => {
                using ( var server = HttpHelper.CreateHttpServer() ) {

                    server.StartListening();
                    server.StopListening();
                    server.StopListening();

                }
            });

        }
        
    }

}