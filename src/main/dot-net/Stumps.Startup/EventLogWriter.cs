namespace Stumps
{
    /// <summary>
    ///     A class that provides an implementation of <see cref="T:Stumps.IMessageWriter"/> that writes messages
    ///     to the Windows event logs.
    /// </summary>
    public class EventLogWriter : IMessageWriter
    {
        /// <summary>
        ///     Records a debug message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void WriteDebug(string message)
        {
        }

        /// <summary>
        ///     Records an error message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void WriteError(string message)
        {
        }

        /// <summary>
        ///     Records an information message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void Information(string message)
        {
        }
    }
}
