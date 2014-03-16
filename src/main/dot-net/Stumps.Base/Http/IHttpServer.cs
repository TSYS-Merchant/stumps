namespace Stumps.Http
{

    using System;

    /// <summary>
    ///     An interface that defines a basic HTTP server.
    /// </summary>
    internal interface IHttpServer : IDisposable
    {

        /// <summary>
        ///     Occurs when an incomming HTTP request is finishing the processing.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestFinishing;

        /// <summary>
        ///     Occurs when an incomming HTTP request begins processing.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestStarting;

        /// <summary>
        ///     Gets TCP port used by the instance to listen for HTTP requests.
        /// </summary>
        /// <value>
        ///     The port used to listen for HTTP requets.
        /// </value>
        int Port { get; }

        /// <summary>
        ///     Gets a value indicating whether the instance is started.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance is started; otherwise, <c>false</c>.
        /// </value>
        bool Started { get; }

        /// <summary>
        ///     Starts the instance listening for HTTP requests.
        /// </summary>
        void StartListening();

        /// <summary>
        ///     Stops the instance from listening for HTTP requests.
        /// </summary>
        void StopListening();

    }

}
