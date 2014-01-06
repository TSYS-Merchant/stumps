namespace Stumps {

    using System.ServiceProcess;

    public class ServiceStartup : IStartup {

        public void RunInstance(string[] args) {

            var servicesToRun = new ServiceBase[] {
                new StumpsService()
            };

            ServiceBase.Run(servicesToRun);

        }

    }

}
