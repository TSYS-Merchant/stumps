namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;

    public sealed class RecordedRequest : IRecordedContextPart {

        public RecordedRequest() {
            this.Headers = new List<HttpHeader>();
        }

        public IList<HttpHeader> Headers { get; set; }

        public string HttpMethod { get; set; }

        public string RawUrl { get; set; }

        public byte[] Body { get; set; }

        public string BodyContentType { get; set; }

        public bool BodyIsImage { get; set; }

        public bool BodyIsText { get; set; }

        public HttpHeader FindHeader(string name) {

            for ( int i = 0; i < this.Headers.Count; i++ ) {
                if ( this.Headers[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase) ) {
                    return this.Headers[i];
                }
            }

            return null;

        }

    }

}
