namespace Stumps
{

    using System;
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
        public void ProcessRequest_ExecuteMultipleHandlersInPipeline()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var pipe = new HttpPipelineHandler();
            var innerHandler1 = Substitute.For<IHttpHandler>();
            var innerHandler2 = Substitute.For<IHttpHandler>();

            innerHandler1.ProcessRequest(Arg.Any<IStumpsHttpContext>()).Returns(ProcessHandlerResult.Continue);
            innerHandler2.ProcessRequest(Arg.Any<IStumpsHttpContext>()).Returns(ProcessHandlerResult.Continue);

            pipe.Add(innerHandler1);
            pipe.Add(innerHandler2);

            var result = pipe.ProcessRequest(context);

            Assert.AreEqual(ProcessHandlerResult.Continue, result, "The process request returned Continue.");
            innerHandler1.ReceivedWithAnyArgs(1).ProcessRequest(null);
            innerHandler2.ReceivedWithAnyArgs(1).ProcessRequest(null);

        }

        [Test]
        public void ProcessRequest_StopsExecutingWhenTerminateReturned()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var pipe = new HttpPipelineHandler();
            var innerHandler1 = Substitute.For<IHttpHandler>();
            var innerHandler2 = Substitute.For<IHttpHandler>();

            innerHandler1.ProcessRequest(Arg.Any<IStumpsHttpContext>()).Returns(ProcessHandlerResult.Terminate);
            innerHandler2.ProcessRequest(Arg.Any<IStumpsHttpContext>()).Returns(ProcessHandlerResult.Continue);

            pipe.Add(innerHandler1);
            pipe.Add(innerHandler2);

            var result = pipe.ProcessRequest(context);

            Assert.AreEqual(ProcessHandlerResult.Terminate, result, "The process request returned a Continue.");
            innerHandler1.ReceivedWithAnyArgs(1).ProcessRequest(null);
            innerHandler2.ReceivedWithAnyArgs(0).ProcessRequest(null);

        }

        [Test]
        public void ProcessRequest_WithNullContext_ThrowsException()
        {

            var pipe = new HttpPipelineHandler();

            Assert.That(
                () => pipe.ProcessRequest(null),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("context"));

        }

        [Test]
        public void ProcessRequest_WithoutHandlers_ReturnsContinue()
        {

            var context = Substitute.For<IStumpsHttpContext>();
            var handler = new HttpPipelineHandler();

            var result = handler.ProcessRequest(context);
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