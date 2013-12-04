namespace Stumps.Proxy {

    using System;

    public class RecordedContext {

        public RecordedContext() {
            this.RequestDate = DateTime.Now;
        }

        public DateTime RequestDate { get; private set; }

        public RecordedRequest Request { get; set; }

        public RecordedResponse Response { get; set; }

    }

}
