namespace Stumps.Server
{

    using System;
    using System.Net;
    using System.Net.Sockets;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps;
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

            int port = NetworkInformation.FindRandomOpenPort();
            string externalHostName = "www.foo.com";
            bool autoStart = false;
            bool useSsl = true;

            var proxyEntity = new ProxyServerEntity
            {
                AutoStart = autoStart,
                ExternalHostName = externalHostName,
                Port = port,
                UseSsl = useSsl,
                ProxyId = Stumps.Server.Utility.RandomGenerator.GenerateIdentifier()
            };

            var dataAccess = Substitute.For<IDataAccess>();
            dataAccess.ProxyServerFind(Arg.Any<string>()).Returns(proxyEntity);

            // create a TcpListener already listening on the port
            var tcpListener = new TcpListener(IPAddress.Loopback, port);

            try 
            {
                tcpListener.Start();

                StumpsHost proxy = new StumpsHost(Substitute.For<IServerFactory>(), Substitute.For<ILogger>(), dataAccess);
                Assert.That(
                    () => proxy.CreateServerInstance(externalHostName, port, useSsl, autoStart),
                    Throws.Exception.TypeOf<StumpsNetworkException>().With.Property("Message").EqualTo("The port is already in use."));
            } 
            finally 
            {
                tcpListener.Stop();
            }

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