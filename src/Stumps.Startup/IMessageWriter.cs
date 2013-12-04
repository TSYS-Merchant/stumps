namespace Stumps {

    using System;

    public interface IMessageWriter {

        void WriteError(string value);

        void WriteMessage(string value);

    }

}
