namespace Stumps
{

    using System;

    public class ConsoleStartup : IStartup
    {

        public StumpsConfiguration Configuration { get; set; }

        public IMessageWriter MessageWriter { get; set; }

        public void RunInstance()
        {

            this.MessageWriter.Information(Resources.StartupStarting);

            using (var server = new StumpsServer(this.Configuration))
            {
                server.Start();
                this.MessageWriter.Information(Resources.StartupComplete);

                Console.ReadLine();

                this.MessageWriter.Information(Resources.ShutdownStarting);
                server.Stop();
                this.MessageWriter.Information(Resources.ShutdownComplete);
            }

        }

    }

}