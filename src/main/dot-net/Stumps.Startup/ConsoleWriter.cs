namespace Stumps
{

    using System;

    public class ConsoleWriter : IMessageWriter
    {

        public void WriteDebug(string message)
        {
            WriteToConsole(message, ConsoleColor.Yellow);
        }

        public void WriteError(string message)
        {
            WriteToConsole(message, ConsoleColor.Red);
        }

        public void Information(string message)
        {
            Console.WriteLine(message);
        }

        private static void WriteToConsole(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }

}