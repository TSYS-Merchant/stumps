namespace Stumps.Proxy
{

    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;

    [TestFixture]
    public class ProxyStumpsTests
    {

        private static string _domain = "www.google.com";

        [Test]
        public void StumpNameExists_WithExistantName_ReturnsTrue()
        {
            var dal = Substitute.For<IDataAccess>();
            StumpContract contract = new StumpContract();
            contract.StumpName = "name";
            contract.StumpId = "abc";
            contract.MatchHeaders = new HttpHeader[]
            {
            };
            contract.Response = new RecordedResponse();

            ProxyStumps stump = new ProxyStumps(_domain, dal);
            stump.CreateStump(contract);
            Assert.IsTrue(stump.StumpNameExists("name"));
        }

        [Test]
        public void StumpNameExists_WithNonExistantName_ReturnsFalse()
        {
            var dal = Substitute.For<IDataAccess>();

            ProxyStumps stump = new ProxyStumps(_domain, dal);
            Assert.IsFalse(stump.StumpNameExists("nameDNE"));
        }

    }

}