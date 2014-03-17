namespace Stumps.Server
{

    /// <summary>
    ///     A class that represents a factory that creates instances of <see cref="T:Stumps.StumpsServer" /> objects.
    /// </summary>
    public class ServerFactory : IServerFactory
    {

        /// <summary>
        ///     Creates a new instance of <see cref="T:Stumps.IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="defaultResponse">The default response returned to a client when a matching <see cref="T:Stumps.Stump"/> is not found.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        /// <returns>An instance of a class inherting from the <see cref="T:Stumps.IStumpsServer"/> interface.</returns>
        public IStumpsServer CreateServer(int listeningPort, ServerDefaultResponse defaultResponse)
        {
            var server = new StumpsServer(listeningPort, defaultResponse);
            return server;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="T:Stumps.IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="proxyHostUri">The external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="listeningPort" /> exceeds the allowed TCP port range.</exception>
        /// <returns>An instance of a class inherting from the <see cref="T:Stumps.IStumpsServer"/> interface.</returns>
        public IStumpsServer CreateServer(int listeningPort, System.Uri proxyHostUri)
        {
            var server = new StumpsServer(listeningPort, proxyHostUri);
            return server;
        }

    }

}
