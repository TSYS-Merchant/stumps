namespace Stumps.Utility
{

    using System.Net.NetworkInformation;
    using NUnit.Framework;

    [TestFixture]
    public class NetworkUtilityTests
    {

        [Test]
        public void FindRandomOpenPort_DiscoveredPortsTendToBeRandom()
        {

            var port1 = NetworkUtility.FindRandomOpenPort();
            var port2 = NetworkUtility.FindRandomOpenPort();
            var port3 = NetworkUtility.FindRandomOpenPort();

            var arePortsEqual = port1 != port2 || port2 != port3 || port1 != port3;
            Assert.IsFalse(arePortsEqual);

        }

        [Test]
        public void FindRandomOpenPort_FindsValidPort()
        {

            var port = NetworkUtility.FindRandomOpenPort();
            Assert.IsFalse(IsPortInUse(port));

        }

        private static bool IsPortInUse(int port)
        {

            var globalIpProperties = IPGlobalProperties.GetIPGlobalProperties();
            var connections = globalIpProperties.GetActiveTcpConnections();

            var inUse = false;

            foreach (var connection in connections)
            {
                if (connection.LocalEndPoint.Port == port)
                {
                    inUse = true;
                    break;
                }
            }

            return inUse;

        }

    }

}