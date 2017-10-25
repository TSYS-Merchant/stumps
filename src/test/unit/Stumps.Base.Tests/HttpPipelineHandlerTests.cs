namespace Stumps
{

    using System;
    using System.Threading.Tasks;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Http;

    [TestFixture]
    public class HttpPipelineHandlerTests
    {

        [Test]
        public void AddHandler_AcceptsMultipleHandlers()
        {

            var pipe = new HttpPipelineHandler();
            Assert.AreEqual(0, pipe.Count);

            pipe.Add(Substitute.For<IHttpHandler>());
            pipe.Add(Substitute.For<IHttpHandler>());
            Assert.AreEqual(2, pipe.Count);

        }

        [Test]
        public void Indexer_ReturnsItem()
        {

            var pipe = new HttpPipelineHandler();
            
            var handler1 = new MockHandler()
            {
                HandlerId = 1
            };
            
            var handler2 = new MockHandler()
            {
                HandlerId = 2
            };

            pipe.Add(handler1);
            pipe.Add(handler2);

            Assert.AreEqual(1, ((MockHandler)pipe[0]).HandlerId);
            Assert.AreEqual(2, ((MockHandler)pipe[1]).HandlerId);

        }

        [Test]
        public async Task ProcessRequest_ExecuteMultipleHandlersInPipeline()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var pipe = new HttpPipelineHandler();
            var innerHandler1 = new NoOpHandler(ProcessHandlerResult.Continue);
            var innerHandler2 = new NoOpHandler(ProcessHandlerResult.Continue);

            pipe.Add(innerHandler1);
            pipe.Add(innerHandler2);

            var result = await pipe.ProcessRequest(context);

            Assert.AreEqual(ProcessHandlerResult.Continue, result, "The process request returned Continue.");
            Assert.AreEqual(1, innerHandler1.ProcessRequestCalls());
            Assert.AreEqual(1, innerHandler2.ProcessRequestCalls());

        }

        [Test]
        public async Task ProcessRequest_StopsExecutingWhenTerminateReturned()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var pipe = new HttpPipelineHandler();
            var innerHandler1 = new NoOpHandler(ProcessHandlerResult.Terminate);
            var innerHandler2 = new NoOpHandler(ProcessHandlerResult.Continue);

            pipe.Add(innerHandler1);
            pipe.Add(innerHandler2);

            var result = await pipe.ProcessRequest(context);

            Assert.AreEqual(ProcessHandlerResult.Terminate, result, "The process request returned a Continue.");
            Assert.AreEqual(1, innerHandler1.ProcessRequestCalls());
            Assert.AreEqual(0, innerHandler2.ProcessRequestCalls());

        }

        [Test]
        public void ProcessRequest_WithNullContext_ThrowsException()
        {

            var pipe = new HttpPipelineHandler();

            Assert.That(
                async () => await pipe.ProcessRequest(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));

        }

        [Test]
        public async Task ProcessRequest_WithoutHandlers_ReturnsContinue()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var handler = new HttpPipelineHandler();

            var result = await handler.ProcessRequest(context);
            Assert.AreEqual(ProcessHandlerResult.Continue, result, "The process request returned a Terminate.");

        }

        [Test]
        public void ProcessRequest_WithValidContext_RaisesContextProcessedEvent()
        {

            var context = Substitute.For<IStumpsHttpContext>();

            var handler = new HttpPipelineHandler();
            var hitCount = 0;
            handler.ContextProcessed += (o, e) => hitCount++;

            var result = handler.ProcessRequest(context);

            Assert.AreEqual(1, hitCount);

        }

    }

}