namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     An interface that provides the ability to create new Stumps servers.
    /// </summary>
    public interface IServerFactory
    {

        /// <summary>
        ///     Creates a new instance of <see cref="T:Stumps.IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="fallbackResponse">The default response returned to a client when a matching <see cref="T:Stumps.Stump"/> is not found.</param>
        /// <returns>An instance of a class inherting from the <see cref="T:Stumps.IStumpsServer"/> interface.</returns>
        IStumpsServer CreateServer(int listeningPort, FallbackResponse fallbackResponse);

        /// <summary>
        ///     Creates a new instance of <see cref="T:Stumps.IStumpsServer" />.
        /// </summary>
        /// <param name="listeningPort">The port the HTTP server is using to listen for traffic.</param>
        /// <param name="remoteServerUri">The URI for the remote server that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.</param>
        /// <returns>An instance of a class inherting from the <see cref="T:Stumps.IStumpsServer"/> interface.</returns>
        IStumpsServer CreateServer(int listeningPort, Uri remoteServerUri);

    }

}
