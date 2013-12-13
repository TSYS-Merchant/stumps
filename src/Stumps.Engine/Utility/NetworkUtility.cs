namespace Stumps.Utility {

    using System;
    using System.Net.NetworkInformation;

    internal static class NetworkUtility {

        public const int MinimumPort = 7000;
        public const int MaximumPort = 10000;

        public static int FindRandomOpenPort() {

            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var endpointList = properties.GetActiveTcpListeners();

            var usedPorts = new bool[NetworkUtility.MaximumPort - NetworkUtility.MinimumPort + 1];

            foreach ( var endpoint in endpointList ) {
                if ( endpoint.Port >= NetworkUtility.MinimumPort && endpoint.Port <= NetworkUtility.MaximumPort ) {
                    var port = endpoint.Port - NetworkUtility.MinimumPort;
                    usedPorts[port] = true;
                }
            }

            var rnd = new Random();

            var foundPort = -1;

            // Maximum 100 tries for sanity
            for ( int i = 0; i < 100; i++ ) {
                var portGuess = rnd.Next(usedPorts.Length - 1);

                if ( !usedPorts[portGuess] ) {
                    foundPort = portGuess + NetworkUtility.MinimumPort;
                    break;
                }
            }

            return foundPort;

        }

    }

}
