namespace Stumps
{
    using System;
    using Stumps.Server;

    /// <summary>
    ///     Provides a Windows console application environment for the Stumps server. 
    /// </summary>
    public class ConsoleStartup : IStartup
    {
        /// <summary>
        ///     Gets or sets the configuration for the Stumps server.
        /// </summary>
        /// <value>
        ///     The configuration for the Stumps server.
        /// </value>
        public StumpsConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the <see cref="T:Stumps.IMessageWriter" /> used to record startup messages.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IMessageWriter" /> used to record startup messages.
        /// </value>
        public IMessageWriter MessageWriter
        {
            get;
            set;
        }

        /// <summary>
        ///     Runs the instance of the Stumps server.
        /// </summary>
        public void RunInstance()
        {
            this.MessageWriter.Information(StartupResources.StartupStarting);

            using (var server = new StumpsRunner(this.Configuration))
            {
                server.Start();
                this.MessageWriter.Information(StartupResources.StartupComplete);

                Console.ReadLine();

                this.MessageWriter.Information(StartupResources.ShutdownStarting);
                server.Shutdown();
                this.MessageWriter.Information(StartupResources.ShutdownComplete);
            }
        }
    }
}
