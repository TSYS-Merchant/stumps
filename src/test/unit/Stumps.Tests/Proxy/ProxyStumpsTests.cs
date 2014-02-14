namespace Stumps.Proxy
{

    using System;
    using NSubstitute;
    using NUnit.Framework;
    using Stumps.Data;
    using Stumps.Logging;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    [TestFixture]
    class ProxyStumpsTests
    {
        
        private static string Domain = "www.google.com";

        [Test]
        public void StumpNameExists_WithNonExistantName_ReturnsFalse()
        {
            var dal = Substitute.For<IDataAccess>();

            ProxyStumps stump = new ProxyStumps(Domain, dal);
            Assert.IsFalse(stump.StumpNameExists("nameDNE"));
        }

        [Test]
        public void StumpNameExists_WithExistantName_ReturnsTrue()
        {
            var dal = Substitute.For<IDataAccess>();
            StumpContract contract = new StumpContract();
            contract.StumpName = "name";
            contract.StumpId = "abc";
            contract.MatchHeaders = new HttpHeader[] { };
            contract.Response = new RecordedResponse();

            ProxyStumps stump = new ProxyStumps(Domain, dal);
            stump.CreateStump(contract);
            Assert.IsTrue(stump.StumpNameExists("name"));
        }

    }

}
