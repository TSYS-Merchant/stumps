namespace Stumps {

    using System.ServiceProcess;

    partial class StumpsService : ServiceBase {

        private readonly StumpsServer _server;

        public StumpsService() {
            InitializeComponent();
            _server = new StumpsServer();
        }

        protected override void OnStart(string[] args) {
            _server.Start();
        }

        protected override void OnStop() {
            _server.Stop();
        }

    }

}
