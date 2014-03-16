namespace Stumps.Server
{

    using System;

    /// <summary>
    ///     An interface that represents a module in the Stumps server.
    /// </summary>
    internal interface IStumpModule : IDisposable
    {

        /// <summary>
        ///     Starts the instance of the module.
        /// </summary>
        void Start();

        /// <summary>
        ///     Shuts down the instance of the module.
        /// </summary>
        void Shutdown();

    }

}