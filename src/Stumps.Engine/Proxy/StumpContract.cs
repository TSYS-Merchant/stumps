namespace Stumps.Proxy {

    public class StumpContract {

        public string HttpMethod { get; set; }

        public byte[] MatchBody { get; set; }

        public string MatchBodyContentType { get; set; }

        public bool MatchBodyIsText { get; set; }

        public bool MatchBodyIsImage { get; set; }

        public int MatchBodyMinimumLength { get; set; }

        public int MatchBodyMaximumLength { get; set; }

        public string[] MatchBodyText { get; set; }

        public HttpHeader[] MatchHeaders { get; set; }

        public bool MatchHttpMethod { get; set; }

        public bool MatchRawUrl { get; set; }

        public string RawUrl { get; set; }

        public RecordedResponse Response { get; set; }

        public string StumpId { get; set; }

        public string StumpName { get; set; }

        public string StumpCategory { get; set; }

    }

}
