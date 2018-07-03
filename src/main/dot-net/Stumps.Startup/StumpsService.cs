namespace Stumps
{
    using System;
    using System.ServiceProcess;
    using Stumps.Server;

    /// <summary>
    ///      A class that implements the Stumps server as a Windows service.
    /// </summary>
    public partial class StumpsService : ServiceBase
    {
        private readonly StumpsRunner _server;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.StumpsService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="configuration"/> is <c>null</c>.</exception>
        public StumpsService(StumpsConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            InitializeComponent();
            _server = new StumpsRunner(configuration);
        }

        /// <summary>
        ///     Executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            _server.Start();
        }

        /// <summary>
        ///     Executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            _server.Shutdown();
        }
    }
}
