namespace Stumps.Server
{

    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Server.Data;

    [TestFixture]
    public class StumpsServerInstanceTests
    {

        private IDataAccess _dal;
        private string _serverId;
        
        [TestFixtureSetUp]
        public void StumpsServerInstanceTests_SetUp()
        {

            _serverId = "ABCD";

            var proxyEntity = new ProxyServerEntity()
            {
                AutoStart = false,
                ExternalHostName = "stumps-project.com",
                Port = 9000,
                ProxyId = _serverId,
                UseSsl = false
            };

            _dal = Substitute.For<IDataAccess>();
            _dal.ProxyServerFind("ABCD").Returns(proxyEntity);

        }

        [Test]
        public void StumpNameExists_WithExistantName_ReturnsTrue()
        {

            var contract = new StumpContract()
            {
                StumpName = "StumpName",
                StumpId = "abc"
            };

            var instance = new StumpsServerInstance(Substitute.For<IServerFactory>(), _serverId, _dal);
            instance.CreateStump(contract);
            Assert.IsTrue(instance.StumpNameExists(contract.StumpName));

        }

        [Test]
        public void StumpNameExists_WithNonExistantName_ReturnsFalse()
        {

            var instance = new StumpsServerInstance(Substitute.For<IServerFactory>(), _serverId, _dal);
            Assert.IsFalse(instance.StumpNameExists("name"));

        }

    }

}
