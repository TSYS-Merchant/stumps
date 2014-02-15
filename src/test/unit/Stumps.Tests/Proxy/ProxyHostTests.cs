namespace Stumps.Proxy
{

    using System;
    using System.Net;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;

    [TestFixture]
    public class ProxyHostTests
    {

        private readonly int _defaultPort;

        public ProxyHostTests()
        {

            var randomGenerator = new Random();
            _defaultPort = randomGenerator.Next(IPEndPoint.MinPort, IPEndPoint.MinPort + 1);

        }

        [Test]
        public void Constructor_NullHostName_ThrowsException()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy(null, _defaultPort, true, false),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("externalHostName"));
        }

        [Test]
        public void Constructor_PortInUse_ThrowsException()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy("www.foo.com", 135, true, false),
                Throws.Exception.TypeOf<StumpsNetworkException>().With.Property("Message").EqualTo("Port is in use"));
        }

        [Test]
        public void Constructor_PortNumberRange_ThrowsException()
        {
            ProxyHost proxy = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateProxy("www.foo.com", IPEndPoint.MaxPort + 1, true, false),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));

            Assert.That(
                () => proxy.CreateProxy("www.foo.com", IPEndPoint.MinPort - 1, true, false),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));
        }

    }

}