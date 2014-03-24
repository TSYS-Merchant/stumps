namespace Stumps
{

    using System;
    using System.Net;
    using System.Net.NetworkInformation;

    /// <summary>
    ///     A class that represents a set of Network related functions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "The name is preferred and should not cause any real conflicts when being used.")]
    public static class NetworkInformation
    {

        public const int MinimumPort = 7000;
        public const int MaximumPort = 10000;

        /// <summary>
        ///     Finds a random open port.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Int32"/> representing an available TCP port.
        /// </returns>
        public static int FindRandomOpenPort()
        {

            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endpointList = properties.GetActiveTcpListeners();

            var usedPorts = new bool[NetworkInformation.MaximumPort - NetworkInformation.MinimumPort + 1];

            foreach (var endpoint in endpointList)
            {
                if (endpoint.Port >= NetworkInformation.MinimumPort &&
                    endpoint.Port <= NetworkInformation.MaximumPort)
                {
                    var port = endpoint.Port - NetworkInformation.MinimumPort;
                    usedPorts[port] = true;
                }
            }

            var rnd = new Random();

            var foundPort = -1;

            // Maximum 100 tries for sanity
            for (int i = 0; i < 100; i++)
            {
                var portGuess = rnd.Next(usedPorts.Length - 1);

                if (!usedPorts[portGuess])
                {
                    foundPort = portGuess + NetworkInformation.MinimumPort;
                    break;
                }
            }

            return foundPort;

        }

        /// <summary>
        ///     Determines whether the specified port is being used.
        /// </summary>
        /// <param name="localPort">The local port.</param>
        /// <returns>
        ///     <c>true</c> if the port is being used; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPortBeingUsed(int localPort)
        {

            if (localPort < IPEndPoint.MinPort || localPort > IPEndPoint.MaxPort)
            {
                return true;
            }

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] endpointList = properties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in endpointList)
            {
                if (endpoint.Port == localPort)
                {
                    return true;
                }
            }

            return false;

        }

    }

}