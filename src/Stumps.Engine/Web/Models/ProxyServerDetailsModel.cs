namespace Stumps.Web.Models {

    public class ProxyServerDetailsModel : ProxyServerModel {

        public bool IsRunning { get; set; }

        public bool RecordTraffic { get; set; }

        public int RecordCount { get; set; }

        public int RequestsServed { get; set; }

        public int StumpsCount { get; set; }

        public int StumpsServed { get; set; }

        public string ProxyId { get; set; }

    }

}
