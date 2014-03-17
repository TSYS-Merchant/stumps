namespace Stumps.Server
{

    using System;
    using System.Net;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Server.Data;
    using Stumps.Server.Logging;

    [TestFixture]
    public class StumpsHostTests
    {

        private readonly int _defaultPort;

        public StumpsHostTests()
        {

            var randomGenerator = new Random();
            _defaultPort = randomGenerator.Next(IPEndPoint.MinPort, IPEndPoint.MinPort + 1);

        }

        [Test]
        public void Constructor_NullHostName_ThrowsException()
        {
            StumpsHost proxy = new StumpsHost(Substitute.For<IServerFactory>(), Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateServerInstance(null, _defaultPort, true, false),
                Throws.Exception.TypeOf<ArgumentNullException>().With.Property("ParamName").EqualTo("externalHostName"));
        }

        [Test]
        public void Constructor_PortInUse_ThrowsException()
        {
            StumpsHost proxy = new StumpsHost(Substitute.For<IServerFactory>(), Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateServerInstance("www.foo.com", 135, true, false),
                Throws.Exception.TypeOf<StumpsNetworkException>().With.Property("Message").EqualTo("The port is already in use."));
        }

        [Test]
        public void Constructor_PortNumberRange_ThrowsException()
        {
            StumpsHost proxy = new StumpsHost(Substitute.For<IServerFactory>(), Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            Assert.That(
                () => proxy.CreateServerInstance("www.foo.com", IPEndPoint.MaxPort + 1, true, false),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));

            Assert.That(
                () => proxy.CreateServerInstance("www.foo.com", IPEndPoint.MinPort - 1, true, false),
                Throws.Exception.TypeOf<ArgumentOutOfRangeException>().With.Property("ParamName").EqualTo("port"));
        }

    }

}