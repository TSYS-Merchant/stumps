namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;

    public sealed class RecordedResponse : IRecordedContextPart {

        public RecordedResponse() {
            this.Headers = new List<HttpHeader>();
        }

        public IList<HttpHeader> Headers { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public byte[] Body { get; set; }

        public string BodyContentType { get; set; }

        public bool BodyIsImage { get; set; }

        public bool BodyIsText { get; set; }

        public HttpHeader FindHeader(string name) {

            HttpHeader header = null;

            for ( int i = 0; i < this.Headers.Count; i++ ) {
                if ( this.Headers[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase) ) {
                    header = this.Headers[i];
                    return header;
                }
            }

            return header;

        }

    }

}
