namespace Stumps {

    using System;

    public class ConsoleWriter : IMessageWriter {

        public void WriteError(string value) {
            Console.WriteLine(value);
        }

        public void WriteMessage(string value) {
            Console.WriteLine(value);
        }

    }

}
