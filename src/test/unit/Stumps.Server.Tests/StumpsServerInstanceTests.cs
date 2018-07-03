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
        
        [SetUp]
        public void StumpsServerInstanceTests_SetUp()
        {
            _serverId = "ABCD";

            var proxyEntity = new ServerEntity()
            {
                AutoStart = false,
                RemoteServerHostName = "stumps-project.com",
                Port = 9000,
                ServerId = _serverId,
                UseSsl = false
            };

            _dal = Substitute.For<IDataAccess>();
            _dal.ServerFind("ABCD").Returns(proxyEntity);
        }

        [Test]
        public void StumpNameExists_WithExistantName_ReturnsTrue()
        {
            var contract = new StumpContract()
            {
                OriginalRequest = new RecordedRequest(Substitute.For<IStumpsHttpRequest>(), ContentDecoderHandling.DecodeNotRequired),
                OriginalResponse = new RecordedResponse(Substitute.For<IStumpsHttpResponse>(), ContentDecoderHandling.DecodeNotRequired),
                Response = new RecordedResponse(Substitute.For<IStumpsHttpResponse>(), ContentDecoderHandling.DecodeNotRequired),
                StumpCategory = "ABC",
                StumpId = "abc",
                StumpName = "StumpName"
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
