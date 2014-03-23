namespace Stumps.Server.Logging
{

    using System;
    using System.Diagnostics;

    /// <summary>
    ///     A class that logs information to the debug console.
    /// </summary>
    public class DebugLogger : ILogger
    {

        /// <summary>
        ///     Log generic debug information.
        /// </summary>
        /// <param name="data">The data to log.</param>
        public void LogInfo(string data)
        {

            if (data != null)
            {
                Debug.WriteLine(Resources.LogInformationPrefix + data);
            }

        }

        /// <summary>
        ///     Logs the occurance of an exception.
        /// </summary>
        /// <param name="location">The location the exception was logged at.</param>
        /// <param name="exception">The exception being logged.</param>
        public void LogException(string location, Exception exception)
        {

            if (location != null && exception != null)
            {
                Debug.WriteLine(Resources.LogExceptionPrefix + "(" + location + ") " + exception.ToString());
            }

        }

    }

}