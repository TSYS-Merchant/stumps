namespace Stumps
{

    public interface IStartup
    {

        StumpsConfiguration Configuration { get; set; }

        IMessageWriter MessageWriter { get; set; }

        void RunInstance();

    }

}