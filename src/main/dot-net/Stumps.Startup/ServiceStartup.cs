namespace Stumps
{

    using System.ServiceProcess;

    public class ServiceStartup : IStartup
    {

        public Configuration Configuration { get; set; }

        public IMessageWriter MessageWriter { get; set; }

        public void RunInstance()
        {

            var servicesToRun = new ServiceBase[]
            {
                new StumpsService(this.Configuration)
            };

            ServiceBase.Run(servicesToRun);

        }

    }

}