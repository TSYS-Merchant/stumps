namespace Stumps
{

    using System;
    using System.ServiceProcess;
    using Stumps.Server;

    public partial class StumpsService : ServiceBase
    {

        private readonly StumpsRunner _server;

        public StumpsService(StumpsConfiguration configuration)
        {

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            InitializeComponent();
            _server = new StumpsRunner(configuration);
        }

        protected override void OnStart(string[] args)
        {
            _server.Start();
        }

        protected override void OnStop()
        {
            _server.Shutdown();
        }

    }

}