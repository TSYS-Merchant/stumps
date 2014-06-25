namespace Stumps
{

    using System;
    using NUnit.Framework;
    using Stumps.Http;

    [TestFixture]
    public class FallbackResponseHandlerTests
    {

        [Test]
        public void Constructor_WithInvalidEnum_ThrowsException()
        {

            var responseMember = (FallbackResponse)5;

            Assert.That(
                () => new FallbackResponseHandler(responseMember),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("response"));

        }

        [Test]
        public void ProcessRequest_With404Fallback_Returns404()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http404NotFound);
            var context = new MockHttpContext();
            handler.ProcessRequest(context);

            Assert.AreEqual(404, context.Response.StatusCode);
            Assert.AreEqual(HttpStatusCodes.GetStatusDescription(404), context.Response.StatusDescription);

        }

        [Test]
        public void ProcessRequest_With503Fallback_Returns503()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http503ServiceUnavailable);
            var context = new MockHttpContext();
            handler.ProcessRequest(context);

            Assert.AreEqual(503, context.Response.StatusCode);
            Assert.AreEqual(HttpStatusCodes.GetStatusDescription(503), context.Response.StatusDescription);

        }

        [Test]
        public void ProcessRequest_WithNullContext_ThrowsException()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http404NotFound);

            Assert.That(
                () => handler.ProcessRequest(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));

        }

        [Test]
        public void ProcessRequest_WithPopulatedResponse_ClearsHeadersAndBody()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http503ServiceUnavailable);
            var context = new MockHttpContext();
            context.Response.AppendToBody(new byte[100]);
            context.Response.Headers["ABCD"] = "123";
            handler.ProcessRequest(context);

            Assert.AreEqual(0, context.Response.Headers.Count);
            Assert.AreEqual(0, context.Response.BodyLength);

        }

        [Test]
        public void ProcessRequest_WithValidContext_RaisesContextProcessedEvent()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http503ServiceUnavailable);

            var hitCount = 0;
            handler.ContextProcessed += (o, e) => hitCount++;

            var context = new MockHttpContext();

            handler.ProcessRequest(context);
            Assert.AreEqual(1, hitCount);

        }

        [Test]
        public void ProcessRequest_WithStumpsHttpResponse_PopulatesOrigin()
        {

            var handler = new FallbackResponseHandler(FallbackResponse.Http503ServiceUnavailable);

            var hitCount = 0;
            handler.ContextProcessed += (o, e) => hitCount++;

            var response = new StumpsHttpResponse();
            var context = new MockHttpContext(null, response);

            handler.ProcessRequest(context);
            Assert.AreEqual(response.Origin, HttpResponseOrigin.ServiceUnavailable);

        }

    }

}
