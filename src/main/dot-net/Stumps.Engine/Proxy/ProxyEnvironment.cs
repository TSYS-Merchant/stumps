namespace Stumps.Proxy
{

    using System.Threading;
    using Stumps.Data;

    public class ProxyEnvironment
    {

        private IDataAccess _dataAccess;
        private int _requestsServed;
        private int _stumpsServed;

        public ProxyEnvironment(string proxyId, IDataAccess dataAccess)
        {
            this.Recordings = new ProxyRecordings();
            this.Stumps = new ProxyStumps(proxyId, dataAccess);

            _dataAccess = dataAccess;
            this.ProxyId = proxyId;
        }

        public bool AutoStart { get; set; }

        public string ExternalHostName { get; set; }

        public bool IsRunning { get; set; }

        public int Port { get; set; }

        public ProxyRecordings Recordings { get; private set; }

        public bool RecordTraffic { get; set; }

        public ProxyStumps Stumps { get; private set; }

        public int StumpsServed
        {
            get { return _stumpsServed; }
        }

        public int RequestsServed
        {
            get { return _requestsServed; }
        }

        public string ProxyId { get; set; }

        public bool UseSsl { get; set; }

        public void IncrementRequestsServed()
        {
            Interlocked.Increment(ref _requestsServed);
        }

        public void IncrementStumpsServed()
        {
            Interlocked.Increment(ref _stumpsServed);
        }

    }

}