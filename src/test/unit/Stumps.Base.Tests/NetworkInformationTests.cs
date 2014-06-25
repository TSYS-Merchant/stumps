namespace Stumps
{

    using System;
    using System.Net;
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

        [Test]
        public void IsPortInUse_With135_ReturnsTrue()
        {
            // This is a really crappy test and only works on Windows
            // environments -- my apologies.

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Assert.IsTrue(NetworkInformation.IsPortBeingUsed(135));
            }
            else
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void IsPortInUse_OutsideOfRange_ReturnsTrue()
        {
            Assert.IsTrue(NetworkInformation.IsPortBeingUsed(IPEndPoint.MinPort - 1));
            Assert.IsTrue(NetworkInformation.IsPortBeingUsed(IPEndPoint.MaxPort + 1));
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