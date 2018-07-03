namespace Stumps.Web.ViewModules
{
    using System;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides helper functions for the view models. 
    /// </summary>
    internal static class ModuleHelper
    {
        /// <summary>
        ///     Returns a string value depending on the state of a <see cref="T:Stumps.Server.StumpsServerInstance"/>.
        /// </summary>
        /// <param name="serverInstance">The instance of the Stumps server.</param>
        /// <param name="running">The message to use if the proxy environment is running.</param>
        /// <param name="stopped">The message to use if the proxy environment is stopped.</param>
        /// <param name="recording">The message to use if the proxy environment is recording.</param>
        /// <returns>A <see cref="T:System.String"/> representing the state of the server.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="serverInstance"/> is <c>null</c>.</exception>
        public static string StateValue(StumpsServerInstance serverInstance, string running, string stopped, string recording)
        {
            serverInstance = serverInstance ?? throw new ArgumentNullException(nameof(serverInstance));

            var value = stopped;

            if (serverInstance.IsRunning && serverInstance.RecordTraffic)
            {
                value = recording;
            }
            else if (serverInstance.IsRunning && !serverInstance.RecordTraffic)
            {
                value = running;
            }

            return value;
        }
    }
}