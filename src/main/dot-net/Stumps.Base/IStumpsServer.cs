namespace Stumps
{
    using System;

    /// <summary>
    ///     An interface that represents a Stumps server.
    /// </summary>
    public interface IStumpsServer : IDisposable
    {
        /// <summary>
        ///     Occurs when the server processed an incoming HTTP request and returned the response to the client.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestFinished;

        /// <summary>
        ///     Occurs after the server has finished processing the HTTP request, 
        ///     and has constructed a response, but before it returned to the client.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestProcessed;

        /// <summary>
        ///     Occurs when the server receives an incoming HTTP request.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestReceived;

        /// <summary>
        ///     Gets or sets the default response when a <see cref="T:Stumps.Stump"/> is not found, 
        ///     and a remote HTTP server is not available.
        /// </summary>
        /// <value>
        ///     The default response when a <see cref="T:Stumps.Stump"/> is not found, and a remote HTTP 
        ///     server is not available.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        FallbackResponse DefaultResponse { get; set; }

        /// <summary>
        ///     Gets a value indicating whether the server is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the server is running; otherwise, <c>false</c>.
        /// </value>
        bool IsRunning { get; }

        /// <summary>
        ///     Gets the port the HTTP server is using to listen for traffic.
        /// </summary>
        /// <value>
        ///     The port the HTTP server is using to listen for traffic.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The value is not a valid TCP port.</exception>
        int ListeningPort { get; }

        /// <summary>
        ///     Gets or sets the remote HTTP that is contacted when a <see cref="T:Stumps.Stump" /> is unavailable to handle the incoming request.
        /// </summary>
        /// <value>
        ///     The remote HTTP that is contacted when a <see cref="T:Stumps.Stump" /> is unavailable to handle the incoming request.
        /// </value>
        /// <exception cref="System.InvalidOperationException">The value cannot be changed while the server is running.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">The URI for the remote HTTP server is invalid.</exception>
        Uri RemoteHttpServer { get; set; }

        /// <summary>
        ///     Gets the number of requests served by the remote host.
        /// </summary>
        /// <value>
        ///     The number of requests served by the remote host.
        /// </value>
        int RequestsServedByRemoteHost { get; }

        /// <summary>
        ///     Gets the number requests served with a Stump.
        /// </summary>
        /// <value>
        ///     The number of requests served with a Stumps.
        /// </value>
        int RequestsServedWithStump { get; }

        /// <summary>
        /// Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        /// The count of Stumps in the collection.
        /// </value>
        int StumpCount { get; }

        /// <summary>
        /// Gets or sets a value indicating whether to use stumps when serving requests.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use stumps when serving requests; otherwise, <c>false</c>.
        /// </value>
        bool StumpsEnabled { get; set; }

        /// <summary>
        ///     Gets the total number of requests served.
        /// </summary>
        /// <value>
        ///     The total number of requests served.
        /// </value>
        int TotalRequestsServed { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether use HTTPS for incoming connections rather than HTTP.
        /// </summary>
        /// <value>
        ///     <c>true</c> to use HTTPS for incoming HTTP connections rather than HTTP.
        /// </value>
        bool UseHttpsForIncomingConnections { get; set; }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump" /> with a specified identifier to the collection.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump" />.</param>
        /// <returns>A new <see cref="T:Stumps.Stump"/> with the specified <paramref name="stumpId"/>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpId"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A <see cref="T:Stumps.Stump" /> with the same identifier already exists.</exception>
        Stump AddNewStump(string stumpId);

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump" /> to the collection.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump" /> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A <see cref="T:Stumps.Stump" /> with the same identifier already exists.</exception>
        void AddStump(Stump stump);

        /// <summary>
        ///     Deletes the specified stump from the collection.
        /// </summary>
        /// <param name="stumpId">The  unique identifier for the stump to remove.</param>
        void DeleteStump(string stumpId);

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> with the specified <paramref name="stumpId"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stump is not found.
        /// </remarks>
        Stump FindStump(string stumpId);

        /// <summary>
        ///     Stops this instance of the Stumps server.
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     Starts this instance of the Stumps server.
        /// </summary>
        void Start();
    }
}
