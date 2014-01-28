namespace Stumps.Proxy {

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;
    using Stumps.Utility;
    using System.Net.NetworkInformation; 

    [TestFixture]
    class ProxyHostTests
    {
        private static Random randomPort = new Random();
        private int DEFAULT_PORT = randomPort.Next(System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MinPort + 1);

        [Test]
        public void Constructor_NullHostName_ThrowsException ()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(),Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy(null, DEFAULT_PORT, true, false),
                Throws.Exception
                    .TypeOf<ArgumentNullException>()
                    .With.Property("ParamName")
                    .EqualTo("externalHostName")
            );
        }

        [Test]
        public void Constructor_PortNumberRange_ThrowsException()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy("www.foo.com", (System.Net.IPEndPoint.MaxPort + 1), true, false),
                Throws.Exception
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName")
                    .EqualTo("port")
            );

            Assert.That(
                () => proxy.CreateProxy("www.foo.com", (System.Net.IPEndPoint.MinPort - 1), true, false),
                Throws.Exception
                    .TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName")
                    .EqualTo("port")
            );
        }

        [Test]
        public void Constructor_PortInUse_ThrowsException()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy("www.foo.com", 135, true, false),
                Throws.Exception
                    .TypeOf<PortInUseException>()
                    .With.Property("Message")
                    .EqualTo("port")
            );
        }
    }
}
