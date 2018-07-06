namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class SequencedHttpResponseFactoryTests
    {
        [Test]
        public void Constructor_WithBehavior_InitializesCorrectDefaults()
        {
            const int NotImplementedStatusCode = 501;

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);

            Assert.AreEqual(SequenceBehavior.Random, factory.Behavior);
            Assert.AreEqual(0, factory.Count);
            Assert.NotNull(factory.DefaultResponse);
            Assert.AreEqual(NotImplementedStatusCode, factory.DefaultResponse.StatusCode);
        }

        [Test]
        public void Constructor_WithDefaultResponse_SetsResponse()
        {
            const int NotFoundStatusCode = 404;

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, HttpErrorResponses.HttpNotFound);

            Assert.AreEqual(SequenceBehavior.Random, factory.Behavior);
            Assert.AreEqual(0, factory.Count);
            Assert.NotNull(factory.DefaultResponse);
            Assert.AreEqual(NotFoundStatusCode, factory.DefaultResponse.StatusCode);
        }

        [Test]
        public void Constructor_WithNullDefaultResponse_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    IStumpsHttpResponse nullResponse = null;
                    var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, nullResponse);
                },
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("defaultResponse"));
        }

        [Test]
        public void Constructor_WithNullEnumerableResponses_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    IEnumerable<IStumpsHttpResponse> nullResponse = null;
                    var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, nullResponse);
                },
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("responses"));
        }

        [Test]
        public void Constructor_WithEmptyEnumerableResponses_AddsResponses()
        {
            var responses = new List<IStumpsHttpResponse>();

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, responses);

            Assert.AreEqual(SequenceBehavior.Random, factory.Behavior);
            Assert.AreEqual(responses.Count, factory.Count);
        }


        [Test]
        public void Constructor_WithMultipleEnumerableResponses_AddsResponses()
        {
            var responses = new List<IStumpsHttpResponse>
            {
                new BasicHttpResponse(),
                new BasicHttpResponse(),
                new BasicHttpResponse()
            };

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, responses);

            Assert.AreEqual(SequenceBehavior.Random, factory.Behavior);
            Assert.AreEqual(responses.Count, factory.Count);
        }

        [Test]
        public void Indexor_WithInvalidIndex_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);

            Assert.Throws<ArgumentOutOfRangeException>(() => factory[0].GetBody());
        }

        [Test]
        public void Indexor_WhenFactoryIsDisposed_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Dispose();

            Assert.Throws<ObjectDisposedException>(() => factory[0].GetBody());
        }

        [Test]
        public void Indexor_WithValidIndex_ReturnsCorrectValue()
        {
            var responses = new List<IStumpsHttpResponse>();
            responses.Add(new BasicHttpResponse());

            responses.Add(new BasicHttpResponse()
            {
                StatusCode = 1
            });

            responses.Add(new BasicHttpResponse());

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random, responses);

            Assert.AreSame(responses[1], factory[1]);
            Assert.AreNotSame(responses[0], factory[1]);
        }

        [Test]
        public void HasResponse_AlwaysTrue()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            Assert.IsTrue(factory.HasResponse);
        }

        [Test]
        public void Add_WhenFactoryIsDisposed_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Dispose();

            Assert.Throws<ObjectDisposedException>(() => factory.Add(new BasicHttpResponse()));
        }

        [Test]
        public void Add_WithNullResponse_ThrowsException()
        {
            Assert.That(
                () =>
                {
                    var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
                    factory.Add(null);
                },
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("response"));
        }

        [Test]
        public void Add_WhenCalled_ResetsPosition()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 3);

            Assert.AreEqual(1, factory.CreateResponse(new MockHttpRequest()).StatusCode);
            Assert.AreEqual(2, factory.CreateResponse(new MockHttpRequest()).StatusCode);

            factory.Add(new BasicHttpResponse
            {
                StatusCode = 4
            });

            Assert.AreEqual(1, factory.CreateResponse(new MockHttpRequest()).StatusCode);
        }

        [Test]
        public void Add_WithValidResponse_AddsToCollection()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Add(new BasicHttpResponse());

            Assert.AreEqual(1, factory.Count);
        }

        [Test]
        public void Add_WithTightThread_IsReasonablyWriteSafe()
        {
            const int ItemCount = 1000;

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);

            var t = new Thread(() =>
            {
                Parallel.For(0, ItemCount, (i) =>
                {
                    var response = new BasicHttpResponse()
                    {
                        StatusCode = i
                    };

                    factory.Add(response);
                });
            });

            t.Start();
            Assert.IsTrue(t.Join(5000));

            if (t.IsAlive)
            {
                t.Abort();
            }

            Assert.AreEqual(ItemCount, factory.Count);
        }

        [Test]
        public void Clear_WhenCalled_ResetsPosition()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 3);

            Assert.AreEqual(1, factory.CreateResponse(new MockHttpRequest()).StatusCode);
            Assert.AreEqual(2, factory.CreateResponse(new MockHttpRequest()).StatusCode);

            factory.Clear();

            factory.Add(new BasicHttpResponse
            {
                StatusCode = 4
            });

            Assert.AreEqual(4, factory.CreateResponse(new MockHttpRequest()).StatusCode);
        }

        [Test]
        public void Clear_WhenFactoryIsDisposed_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Dispose();

            Assert.Throws<ObjectDisposedException>(() => factory.Clear());
        }

        [Test]
        public void Clear_WhenCalled_RemoveAllItemsInCollection()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Add(new BasicHttpResponse());

            factory.Clear();

            Assert.AreEqual(0, factory.Count);
        }

        [Test]
        public void Clear_WithTightThread_IsReasonablyWriteSafe()
        {
            var t = new Thread(() =>
            {
                var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
                Assert.DoesNotThrow(() => Parallel.For(0, 1000, (i) => factory.Clear()));
            });

            t.Start();
            Assert.IsTrue(t.Join(1000));

            if (t.IsAlive)
            {
                t.Abort();
            }
        }

        [Test]
        public void CreateResponse_WhenFactoryIsDisposed_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Dispose();

            Assert.Throws<ObjectDisposedException>(() => factory.CreateResponse(new MockHttpRequest()));
        }

        [Test]
        public void CreateResponse_WithoutResponsesWithRandomBehavior_ReturnsDefault()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            var response = factory.CreateResponse(new MockHttpRequest());

            Assert.NotNull(response);
            Assert.AreEqual(factory.DefaultResponse.StatusCode, response.StatusCode);
        }

        [Test]
        public void CreateResponse_WithoutResponsesWithLoopBehavior_ReturnsDefault()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            var response = factory.CreateResponse(new MockHttpRequest());

            Assert.NotNull(response);
            Assert.AreEqual(factory.DefaultResponse.StatusCode, response.StatusCode);
        }

        [Test]
        public void CreateResponse_WithoutResponsesWithReturnDefaultBehavior_ReturnsDefault()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenDefault);
            var response = factory.CreateResponse(new MockHttpRequest());

            Assert.NotNull(response);
            Assert.AreEqual(factory.DefaultResponse.StatusCode, response.StatusCode);
        }

        [Test]
        public void CreateResponse_WithRandomBehaviorBehavior_ReturnsExpectedValues()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            PopulateFactoryWithMultipleResponses(factory, 5);

            var responses = GetMultipleResponsesFromFactory(factory, 10);

            var unexpectedStatusCodeSequence = new int[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };

            Assert.IsFalse(AreStatusCodesEqual(responses, unexpectedStatusCodeSequence));

            responses.ForEach((r) => Assert.AreNotEqual(factory.DefaultResponse.StatusCode, r.StatusCode));
        }

        [Test]
        public void CreateResponse_WithSequenceThenDefaultBehavior_ReturnsExpectedValues()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenDefault);
            PopulateFactoryWithMultipleResponses(factory, 3);

            var responses = GetMultipleResponsesFromFactory(factory, 5);

            var expectedStatusCodeSequence = new int[] { 1, 2, 3, 501, 501 };

            Assert.IsTrue(AreStatusCodesEqual(responses, expectedStatusCodeSequence));
        }

        [Test]
        public void CreateResponse_WithSequenceThenLoopBehavior_ReturnsExpectedValues()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 5);

            var responses = GetMultipleResponsesFromFactory(factory, 10);

            var expectedStatusCodeSequence = new int[] { 1, 2, 3, 4, 5, 1, 2, 3, 4, 5 };

            Assert.IsTrue(AreStatusCodesEqual(responses, expectedStatusCodeSequence));
        }

        [Test]
        public void CreateResponse_WithLoopBehaviorInMultithreadedEnvironment_ReturnsExpectedValues()
        {
            var counts = new int[21];

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 20);

            var mockRequest = new MockHttpRequest();

            Parallel.For(0, 1000, (i) =>
            {
                var response = factory.CreateResponse(mockRequest);
                Interlocked.Increment(ref counts[response.StatusCode]);
            });

            for (var i = 1; i <= 20; i++)
            {
                Assert.AreEqual(50, counts[i]);
            }
        }

        [Test]
        public void RemoveAt_WhenCalledWithBadPosition_ThrowsException()
        {
            const int BadPosition = 0;

            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            Assert.Throws<ArgumentOutOfRangeException>(() => factory.RemoveAt(BadPosition));
        }

        [Test]
        public void RemoveAt_WhenCalled_ResetsPosition()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 5);

            Assert.AreEqual(1, factory.CreateResponse(new MockHttpRequest()).StatusCode);
            Assert.AreEqual(2, factory.CreateResponse(new MockHttpRequest()).StatusCode);

            factory.RemoveAt(3);

            Assert.AreEqual(1, factory.CreateResponse(new MockHttpRequest()).StatusCode);
        }

        [Test]
        public void RemoveAt_WhenFactoryIsDisposed_ThrowsExpcetion()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.Random);
            factory.Dispose();

            Assert.Throws<ObjectDisposedException>(() => factory.RemoveAt(0));
        }

        [Test]
        public void RemoveAt_WithValidPosition_RemovesResponse()
        {
            var factory = new SequencedHttpResponseFactory(SequenceBehavior.SequentialThenLoop);
            PopulateFactoryWithMultipleResponses(factory, 3);

            factory.RemoveAt(1);

            Assert.AreEqual(2, factory.Count);
            Assert.AreEqual(1, factory[0].StatusCode);
            Assert.AreEqual(3, factory[1].StatusCode);
        }

        private void PopulateFactoryWithMultipleResponses(SequencedHttpResponseFactory factory, int count)
        {
            for (var i = 1; i <= count; i++)
            {
                var response = new BasicHttpResponse()
                {
                    StatusCode = i
                };

                factory.Add(response);
            }
        }

        private List<IStumpsHttpResponse> GetMultipleResponsesFromFactory(SequencedHttpResponseFactory factory, int count)
        {
            var request = new MockHttpRequest();
            var responses = new List<IStumpsHttpResponse>();

            for (var i = 0; i < count; i++)
            {
                responses.Add(factory.CreateResponse(request));
            }

            return responses;
        }

        private bool AreStatusCodesEqual(List<IStumpsHttpResponse> responses, int[] sequence)
        {
            var areEqual = true;

            for (var i = 0; i < sequence.Length; i++)
            {
                areEqual = areEqual & (responses[i].StatusCode == sequence[i]);
            }

            return areEqual;
        }
    }
}
