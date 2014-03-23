namespace Stumps
{

    using System.Net;

    /// <summary>
    ///     An interface that represents an incomming HTTP request.
    /// </summary>
    public interface IStumpsHttpRequest : IStumpsHttpContextPart
    {

        /// <summary>
        ///     Gets the HTTP data transfer method used by the client.
        /// </summary>
        /// <value>
        ///     The HTTP data transfer method used by the client.
        /// </value>
        string HttpMethod { get; }

        /// <summary>
        ///     Gets the local end point where the HTTP request was received on.
        /// </summary>
        /// <value>
        ///     The local end point where the HTTP request was received on.
        /// </value>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        ///     Gets the HTTP protocol version.
        /// </summary>
        /// <value>
        ///     The HTTP protocol version.
        /// </value>
        string ProtocolVersion { get; }

        /// <summary>
        ///     Gets the raw URL of the current request.
        /// </summary>
        /// <value>
        ///     The raw URL of the current request.
        /// </value>
        string RawUrl { get; }

        /// <summary>
        ///     Gets the remote end point the HTTP request came from.
        /// </summary>
        /// <value>
        ///     The remote end point where the HTTP request came from.
        /// </value>
        IPEndPoint RemoteEndPoint { get; }

    }

}