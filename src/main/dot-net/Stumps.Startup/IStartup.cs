namespace Stumps
{

    public interface IStartup
    {

        Configuration Configuration { get; set; }

        IMessageWriter MessageWriter { get; set; }

        void RunInstance();

    }

}