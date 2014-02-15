namespace Stumps
{

    using System;
    using System.ServiceProcess;

    public partial class StumpsService : ServiceBase
    {

        private readonly StumpsServer _server;

        public StumpsService(Configuration configuration)
        {

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            InitializeComponent();
            _server = new StumpsServer(configuration);
        }

        protected override void OnStart(string[] args)
        {
            _server.Start();
        }

        protected override void OnStop()
        {
            _server.Stop();
        }

    }

}