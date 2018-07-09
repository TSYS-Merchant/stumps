namespace Stumps
{
    using System;

    /// <summary>
    ///     A class that provides an implementation of <see cref="IMessageWriter"/> that writes messages
    ///     to the Windows console.
    /// </summary>
    public class ConsoleWriter : IMessageWriter
    {
        /// <summary>
        ///     Records a debug message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void WriteDebug(string message) => WriteToConsole(message, ConsoleColor.Yellow);

        /// <summary>
        ///     Records an error message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void WriteError(string message) => WriteToConsole(message, ConsoleColor.Red);

        /// <summary>
        ///     Records an information message.
        /// </summary>
        /// <param name="message">The message to record.</param>
        public void Information(string message) => Console.WriteLine(message);

        /// <summary>
        ///     Writes the automatic console.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        private static void WriteToConsole(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
