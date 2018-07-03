namespace Stumps
{
    using System.ServiceProcess;
    using Stumps.Server;

    /// <summary>
    ///     Provides a Windows Service environment for the Stumps server. 
    /// </summary>
    public class ServiceStartup : IStartup
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
            var servicesToRun = new ServiceBase[]
            {
                new StumpsService(this.Configuration)
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}
