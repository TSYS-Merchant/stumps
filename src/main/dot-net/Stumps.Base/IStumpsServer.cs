namespace Stumps
{

    using System;

    /// <summary>
    ///     An interface that represents a Stumps server.
    /// </summary>
    public interface IStumpsServer : IDisposable
    {

        /// <summary>
        ///     Occurs when the server finishes processing an HTTP request.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestProcessed;

        /// <summary>
        ///     Occurs when the server receives an incomming HTTP request.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestReceived;

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
        int ListeningPort { get; }

        /// <summary>
        ///     Gets the external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.
        /// </summary>
        /// <value>
        ///     The external host that is contacted when a <see cref="T:Stumps.Stump"/> is unavailable to handle the incomming request.
        /// </value>
        Uri ProxyHostUri { get; }

        /// <summary>
        ///     Gets the number of requests served with the proxy.
        /// </summary>
        /// <value>
        ///     The number of requests served with the proxy.
        /// </value>
        int RequestsServedWithProxy { get; }

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
