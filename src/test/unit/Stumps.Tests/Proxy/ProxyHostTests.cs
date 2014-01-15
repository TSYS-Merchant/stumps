namespace Stumps.Proxy {

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;

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
        public void CreateProxy_WithHttpProtocol_ReturnsSame()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            ProxyEnvironment env = proxy.CreateProxy("http://www.google.com", DEFAULT_PORT, true, false);
            Assert.IsTrue(env.UseSsl);
            Assert.IsFalse(env.AutoStart);
            Assert.AreEqual("www.google.com", env.ExternalHostName);
        }

        [Test]
        public void ContainsProtocol_ReturnsTrue()
        {   
            Assert.IsTrue(ProxyHost.containsProtocol("https://www.google.com"));
            Assert.IsTrue(ProxyHost.containsProtocol("http://www.google.com"));
            Assert.IsTrue(ProxyHost.containsProtocol("HTTPS://www.google.com"));
            Assert.IsTrue(ProxyHost.containsProtocol("HTTP://www.google.com"));
            // need to test for bad data? (e.g., "http://http://"), or is that handled by javascript??
        }

        [Test]
        public void ContainsProtocol_ReturnsFalse()
        {
            Assert.IsFalse(ProxyHost.containsProtocol("www.google.com"));
            Assert.IsFalse(ProxyHost.containsProtocol("htttp://www.google.com"));
        }

        [Test]
        public void IsHttps_ReturnsTrue()
        {
            Assert.IsTrue(ProxyHost.isHttps("https://www.google.com"));
            Assert.IsTrue(ProxyHost.isHttps("HTTPS://www.google.com"));
        }

        [Test]
        public void IsHttps_ReturnsFalse()
        {
            Assert.IsFalse(ProxyHost.isHttps("http://www.google.com"));
            Assert.IsFalse(ProxyHost.isHttps("HTTP://www.google.com"));
            Assert.IsFalse(ProxyHost.isHttps("www.google.com"));
        }

        [Test]
        public void IsValidFullUrl_ReturnsTrue()
        {
            Assert.IsTrue(ProxyHost.isValidFullUrl("http://www.google.com"));
            Assert.IsTrue(ProxyHost.isValidFullUrl("HTTP://www.google.com"));
            Assert.IsTrue(ProxyHost.isValidFullUrl("https://www.google.com"));
            Assert.IsTrue(ProxyHost.isValidFullUrl("HTTPS://www.google.com"));
            Assert.IsTrue(ProxyHost.isValidFullUrl("http://www.google.com:80"));
        }

        [Test]
        public void IsValidFullUrl_ReturnsFalse()
        {
            Assert.IsFalse(ProxyHost.isValidFullUrl("www.google.com"));
            Assert.IsFalse(ProxyHost.isValidFullUrl("http://http://www.foo.com"));
            Assert.IsFalse(ProxyHost.isValidFullUrl("https://https://www.foo.com"));
            Assert.IsFalse(ProxyHost.isValidFullUrl("HTTP://http://www.foo.com"));
            Assert.IsFalse(ProxyHost.isValidFullUrl("HTTPS://http://www.foo.com"));
            Assert.IsFalse(ProxyHost.isValidFullUrl("http//www.foo."));
            Assert.IsFalse(ProxyHost.isValidFullUrl("foo"));
        }
    }
}
