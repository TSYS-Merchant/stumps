using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        private static string DOMAIN = "www.google.com";

        [Test]
        public void CreateStump_UniqueName_CreateStumpSuccessfully()
        {
            ProxyStumps stump = new ProxyStumps(DOMAIN, Substitute.For<IDataAccess>());
            Assert.IsTrue(stump.StumpNameExists("nameDNE"));
        }
    }
}
