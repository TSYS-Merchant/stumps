namespace Stumps.Web.ViewModules
{

    using System;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides helper functions for the view models. 
    /// </summary>
    internal static class ModuleHelper
    {

        /// <summary>
        ///     Returns a string value depending on the state of a <see cref="T:Stumps.Proxy.ProxyEnvironment"/>.
        /// </summary>
        /// <param name="environment">The proxy environment.</param>
        /// <param name="running">The message to use if the proxy environment is running.</param>
        /// <param name="stopped">The message to use if the proxy environment is stopped.</param>
        /// <param name="recording">The message to use if the proxy environment is recording.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="environment"/> is <c>null</c>.</exception>
        public static string StateValue(ProxyEnvironment environment, string running, string stopped, string recording)
        {

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            var value = stopped;

            if (environment.IsRunning && environment.RecordTraffic)
            {
                value = recording;
            }
            else if (environment.IsRunning && !environment.RecordTraffic)
            {
                value = running;
            }

            return value;

        }

    }

}