namespace Stumps.Logging
{

    using System;

    /// <summary>
    ///     An interface for a very basic diagnostic logging.
    /// </summary>
    public interface ILogger
    {

        /// <summary>
        ///     Log generic debug information.
        /// </summary>
        /// <param name="data">The data to log.</param>
        void LogInfo(string data);

        /// <summary>
        ///     Logs the occurance of an exception.
        /// </summary>
        /// <param name="location">The location the exception was logged at.</param>
        /// <param name="exception">The exception being logged.</param>
        void LogException(string location, Exception exception);

    }

}