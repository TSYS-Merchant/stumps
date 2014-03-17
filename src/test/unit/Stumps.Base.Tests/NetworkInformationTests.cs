namespace Stumps
{

    using System.Net.NetworkInformation;
    using NUnit.Framework;

    [TestFixture]
    public class NetworkInformationTests
    {

        [Test]
        public void FindRandomOpenPort_ReturnsPort()
        {
            var port = NetworkInformation.FindRandomOpenPort();
            Assert.IsNotNull(port);

        }

        [Test]
        public void FindRandomOpenPort_FindsValidPort()
        {

            var port = NetworkInformation.FindRandomOpenPort();
            Assert.IsFalse(IsPortInUse(port));

        }

        private static bool IsPortInUse(int port)
        {

            var globalIpProperties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = globalIpProperties.GetActiveTcpConnections();

            var isPortInUse = false;

            foreach (var connection in connections)
            {
                if (connection.LocalEndPoint.Port == port)
                {
                    isPortInUse = true;
                    break;
                }
            }

            return isPortInUse;

        }

    }

}