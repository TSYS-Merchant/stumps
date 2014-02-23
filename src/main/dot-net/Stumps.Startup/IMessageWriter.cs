namespace Stumps
{

    public interface IMessageWriter
    {

        void WriteDebug(string message);

        void WriteError(string message);

        void Information(string message);

    }

}