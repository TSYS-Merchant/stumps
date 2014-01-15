namespace Stumps.Proxy {

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;

    [TestFixture]
    class ProxyHostTests
    {

        [Test]
        public void Constructor_NullHostName_ThrowsException ()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(),Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy(null,7658,true, false),
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
            ProxyEnvironment env = proxy.CreateProxy("http://www.google.com", 7658, true, false);
            Assert.IsTrue(env.UseSsl);
            Assert.IsFalse(env.AutoStart);
            Assert.AreEqual("www.google.com", env.ExternalHostName);
        }
    }
}
