namespace Stumps.Proxy {

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Http;

    [TestFixture]
    public class HttpPipelineHandlerTests {

        /// <summary>
        /// Test to confirm that IHttpHander objects can be successfully added to the 
        /// HttpPipelineHandler collection.
        /// </summary>
        [Test]
        public void AddsMultipleHandlers() {

            var pipe = new HttpPipelineHandler();
            Assert.AreEqual(0, pipe.Count);

            pipe.Add(Substitute.For<IHttpHandler>());
            pipe.Add(Substitute.For<IHttpHandler>());
            Assert.AreEqual(2, pipe.Count);

        }
        
        /// <summary>
        /// Test to confirm that the ProcessRequest method returns Continue when there are no
        /// handlers in the HttpPipelineHandler collection.
        /// </summary>
        [Test]
        public void ReturnsContinueWithNoHandlers() {

            var context = Substitute.For<IStumpsHttpContext>();
            var handler = new HttpPipelineHandler();

            var result = handler.ProcessRequest(context);
            Assert.AreEqual(ProcessHandlerResult.Continue, result, "The process request returned a Terminate.");

        }

        /// <summary>
        /// Test to confirm that if there are multiple handlers in the HttpPipelineHandler IHttpHandler 
        /// collection, the IStumpsHttpContext object is processed through all handlers.
        /// </summary>
        [Test]
        public void ExecutesMultiple() {

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

        /// <summary>
        /// Test to confirm that if a request is unable to be processed by one of the handlers
        /// in the HttpPipelineHandler IHttpHandler collection, the HttpPipelineHandler does
        /// not try to process the request through any other handlers and returns false.
        /// </summary>
        [Test]
        public void StopsExecutingWhenRequested() {

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
        public void ProcessRequestDoesNotAllowNulls() {

            var pipe = new HttpPipelineHandler();

            Assert.That(
                () => pipe.ProcessRequest(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("context")
            );

        }

    }

}