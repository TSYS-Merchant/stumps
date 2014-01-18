namespace Stumps {

    public interface IMessageWriter {

        void Debug(string message);

        void Error(string message);

        void Information(string message);

    }

}
