namespace Stumps
{

    using System.ServiceProcess;
    using Stumps.Server;

    public class ServiceStartup : IStartup
    {

        public StumpsConfiguration Configuration { get; set; }

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