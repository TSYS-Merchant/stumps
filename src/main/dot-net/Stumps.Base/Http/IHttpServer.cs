namespace Stumps.Http
{

    using System;

    /// <summary>
    ///     An interface that defines a basic HTTP server.
    /// </summary>
    internal interface IHttpServer : IDisposable
    {

        /// <summary>
        ///     Occurs when the server processed an incomming HTTP request and returned the response to the client.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestFinished;

        /// <summary>
        ///     Occurs after the server has finished processing the HTTP request, 
        ///     and has constructed a response, but before it returned to the client.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestProcessed;

        /// <summary>
        ///     Occurs when the server receives an incomming HTTP request.
        /// </summary>
        event EventHandler<StumpsContextEventArgs> RequestReceived;

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
