namespace Stumps.Data {

    public class StumpEntity {

        public string HttpMethod { get; set; }

        public string MatchBodyFileName { get; set; }

        public string MatchBodyContentType { get; set; }

        public bool MatchBodyIsText { get; set; }

        public bool MatchBodyIsImage { get; set; }

        public int MatchBodyMinimumLength { get; set; }

        public int MatchBodyMaximumLength { get; set; }

        public string[] MatchBodyText { get; set; }

        public HeaderEntity[] MatchHeaders { get; set; }

        public bool MatchHttpMethod { get; set; }

        public bool MatchRawUrl { get; set; }

        public string RawUrl { get; set; }

        public string ResponseBodyContentType { get; set; }

        public string ResponseBodyFileName { get; set; }

        public bool ResponseBodyIsImage { get; set; }

        public bool ResponseBodyIsText { get; set; }

        public HeaderEntity[] ResponseHeaders { get; set; }

        public int ResponseStatusCode { get; set; }

        public string ResponseStatusDescription { get; set; }

        public string StumpId { get; set; }

        public string StumpName { get; set; }

        public string StumpCategory { get; set; }

    }

}
