namespace Stumps.Proxy
{

    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;

    [TestFixture]
    public class ProxyEnvironmentTests
    {

        [Test]
        public void Constructor_InitializesRecordings()
        {

            var environment = new ProxyEnvironment("ABC", Substitute.For<IDataAccess>());
            Assert.IsNotNull(environment.Recordings);

        }

        [Test]
        public void Constructor_InitializesStumps()
        {

            var environment = new ProxyEnvironment("ABC", Substitute.For<IDataAccess>());
            Assert.IsNotNull(environment.Stumps);

        }

        [Test]
        public void Constructor_InitializesWithProxyId()
        {

            var environment = new ProxyEnvironment("ABC", Substitute.For<IDataAccess>());
            Assert.AreEqual("ABC", environment.ProxyId);

        }

        [Test]
        public void IncrementRequestsServed_IncreasesRequestsServedProperty()
        {

            var environment = new ProxyEnvironment("ABC", Substitute.For<IDataAccess>());

            environment.IncrementRequestsServed();
            environment.IncrementRequestsServed();

            Assert.AreEqual(2, environment.RequestsServed);

        }

        [Test]
        public void IncrementStumpsServed_IncreasesStumpsServedProperty()
        {

            var environment = new ProxyEnvironment("ABC", Substitute.For<IDataAccess>());

            environment.IncrementStumpsServed();
            environment.IncrementStumpsServed();

            Assert.AreEqual(2, environment.StumpsServed);

        }

    }

}