namespace Stumps
{

    using System;

    public class ConsoleStartup : IStartup
    {

        public StumpsConfiguration Configuration { get; set; }

        public IMessageWriter MessageWriter { get; set; }

        public void RunInstance()
        {

            this.MessageWriter.Information(StartupResources.StartupStarting);

            using (var server = new StumpsRunner(this.Configuration))
            {
                server.Start();
                this.MessageWriter.Information(StartupResources.StartupComplete);

                Console.ReadLine();

                this.MessageWriter.Information(StartupResources.ShutdownStarting);
                server.Stop();
                this.MessageWriter.Information(StartupResources.ShutdownComplete);
            }

        }

    }

}