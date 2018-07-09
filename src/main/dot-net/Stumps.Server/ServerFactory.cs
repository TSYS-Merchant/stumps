namespace Stumps.Server
{
    using System;

    /// <summary>
    ///     A class that represents a factory that creates instances of <see cref="StumpsServer" /> objects.
    /// </summary>
    public class ServerFactory : IServerFactory
    {
        /// <summary>
        ///     Creates a new instance of <see cref="IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="fallbackResponse">The default response returned to a client when a matching <see cref="Stump"/> is not found.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        /// <returns>An instance of a class inherting from the <see cref="IStumpsServer"/> interface.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose is taken care of later.")]
        public IStumpsServer CreateServer(int listeningPort, FallbackResponse fallbackResponse)
        {
            var server = new StumpsServer
            {
                ListeningPort = listeningPort,
                DefaultResponse = fallbackResponse
            };

            return server;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="remoteServerUri">The URI for the remote server that is contacted when a <see cref="Stump"/> is unavailable to handle the incoming request.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The URI for the remote HTTP server is invalid.</exception>
        /// <returns>An instance of a class inherting from the <see cref="IStumpsServer"/> interface.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose is taken care of later.")]
        public IStumpsServer CreateServer(int listeningPort, Uri remoteServerUri)
        {
            var server = new StumpsServer
            {
                ListeningPort = listeningPort,
                RemoteHttpServer = remoteServerUri
            };

            return server;
        }
    }
}
