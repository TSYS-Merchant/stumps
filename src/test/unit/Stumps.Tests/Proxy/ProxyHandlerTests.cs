namespace Stumps.Proxy {

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;

    [TestFixture]
    public class ProxyHandlerTests {

        [Test]
        public void Constructor_NullEnvironment_ThowsException() {

            Assert.That(
                () => new ProxyHandler(null, Substitute.For<ILogger>()),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("environment")
            );

        }

        [Test]
        public void Constructor_NullLogger_ThowsException() {

            Assert.That(
                () => new ProxyHandler(new ProxyEnvironment("www.google.com", Substitute.For<IDataAccess>()), null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("logger")
            );

        }

        [Test]
        public void ProcessRequest_NullValue_ThrowsException() {

            var proxy = new ProxyHandler(createTestEnvironment(), Substitute.For<ILogger>());

            Assert.That(
                () => proxy.ProcessRequest(null),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("context")
            );

        }

        private ProxyEnvironment createTestEnvironment() {
            var environment = new ProxyEnvironment("ABCD", Substitute.For<IDataAccess>())
            {
                ExternalHostName = "localhost"
            };

            return environment;
        }

    }

}
