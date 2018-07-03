namespace Stumps
{
    /// <summary>
    ///     An interface that provides the ability to record messages during startup.
    /// </summary>
    public interface IMessageWriter
    {
        /// <summary>
        ///     Records a debug message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        void WriteDebug(string message);

        /// <summary>
        ///     Records an error message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        void WriteError(string message);

        /// <summary>
        ///     Records an information message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        void Information(string message);
    }
}
