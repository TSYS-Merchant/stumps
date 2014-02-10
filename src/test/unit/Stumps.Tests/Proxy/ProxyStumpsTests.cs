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
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    [TestFixture]
    class ProxyStumpsTests
    {
        private static Random randomPort = new Random();
        private static string DOMAIN = "www.google.com";
        private int DEFAULT_PORT = randomPort.Next(System.Net.IPEndPoint.MinPort, System.Net.IPEndPoint.MinPort + 1);

        public StumpContract CreateStumpContract(String name, String id)
        {
            StumpContract contract = new StumpContract();
            contract.HttpMethod = string.Empty;
            contract.MatchBody = new byte[]{};
            contract.MatchBodyContentType = string.Empty;
            contract.MatchBodyIsImage = false;
            contract.MatchBodyIsText = false;
            contract.MatchBodyMaximumLength = 100;
            contract.MatchBodyMinimumLength = 0;
            contract.MatchBodyText = new string[] { string.Empty };
            contract.MatchHeaders = new Stumps.Proxy.HttpHeader[]{};
            contract.MatchHttpMethod = true;
            contract.MatchRawUrl = true;
            contract.RawUrl = "";
            contract.StumpCategory = "Uncategorized";
            contract.StumpId = id;
            contract.StumpName = name;


            return contract;
        }


        

        [Test]
        public void CreateStump_UniqueName_CreateStumpSuccessfully()
        {
            ProxyHost proxyHost = new ProxyHost(Substitute.For<ILogger>(), Substitute.For<IDataAccess>());
            ProxyEnvironment env = proxyHost.CreateProxy(DOMAIN, DEFAULT_PORT, true, true);
            //StumpsHandler stumpsHandler = new StumpsHandler(env, Substitute.For<ILogger>);
            ProxyStumps proxyStump = new ProxyStumps(DOMAIN, Substitute.For<IDataAccess>());

            Assert.IsTrue(proxyStump.CreateStump(CreateStumpContract("test", "123")));
            
            
        }
    }
}
