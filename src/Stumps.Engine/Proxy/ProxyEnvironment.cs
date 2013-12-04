namespace Stumps.Proxy {

    using System.Threading;
    using Stumps.Data;

    public class ProxyEnvironment {

        private int _stumpsServed;
        private int _requestsServed;
        private IDataAccess _dataAccess;

        public ProxyEnvironment(string externalHostName, IDataAccess dataAccess) {
            this.Recordings = new ProxyRecordings();
            this.Stumps = new ProxyStumps(externalHostName, dataAccess);

            _dataAccess = dataAccess;
            this.ExternalHostName = externalHostName;
        }

        public bool AutoStart { get; set; }

        public string ExternalHostName { get; set; }

        public bool IsRunning { get; set; }

        public int Port { get; set; }

        public ProxyRecordings Recordings { get; private set; }

        public bool RecordTraffic { get; set; }

        public ProxyStumps Stumps { get; private set; }

        public int StumpsServed {
            get { return _stumpsServed; }
        }

        public int RequestsServed {
            get { return _requestsServed; }
        }

        public string ProxyId { get; set; }

        public bool UseSsl { get; set; }

        public void IncrementStumpsServed() {
            Interlocked.Increment(ref _stumpsServed);
        }

        public void IncrementRequestsServed() {
            Interlocked.Increment(ref _requestsServed);
        }

    }

}
