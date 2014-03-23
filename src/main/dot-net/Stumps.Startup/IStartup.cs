namespace Stumps
{

    using Stumps.Server;

    public interface IStartup
    {

        StumpsConfiguration Configuration { get; set; }

        IMessageWriter MessageWriter { get; set; }

        void RunInstance();

    }

}