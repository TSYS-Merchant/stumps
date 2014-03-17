namespace Stumps.Server
{

    using NUnit.Framework;
    using NSubstitute;
    using Stumps.Server.Data;
    using Stumps.Server.Proxy;
    using Stumps;

    public class StumpsServerInstanceTests
    {

        private static string _serverId = "www.google.com";

        [Test]
        public void StumpNameExists_WithExistantName_ReturnsTrue()
        {

            //var dal = Substitute.For<IDataAccess>();
            //var contract = new StumpContract();
            //contract.StumpName = "name";
            //contract.StumpId = "abc";
            //contract.MatchHeaders = new HttpHeader[]
            //{
            //};

            //var instance = new StumpsServerInstance(Substitute.For<IServerFactory>(), _serverId, dal);
            //instance.CreateStump(contract);
            //Assert.IsTrue(instance.StumpNameExists("name"));
        }

        [Test]
        public void StumpNameExists_WithNonExistantName_ReturnsFalse()
        {
            //var dal = Substitute.For<IDataAccess>();

            //var instance = new StumpsServerInstance(Substitute.For<IServerFactory>(), _serverId, dal);
            //Assert.IsFalse(instance.StumpNameExists("nameDNE"));
        }

    }

}
