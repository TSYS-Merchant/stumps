namespace Stumps.Web.Models
{

    using System;

    public class RecordingDetailsModel
    {

        public int Index { get; set; }

        public HeaderModel[] RequestHeaders { get; set; }

        public DateTime RequestDate { get; set; }

        public string RequestHttpMethod { get; set; }

        public string RequestRawUrl { get; set; }

        public int RequestBodyLength { get; set; }

        public bool RequestBodyIsImage { get; set; }

        public bool RequestBodyIsText { get; set; }

        public string RequestBody { get; set; }

        public string RequestBodyUrl { get; set; }

        public HeaderModel[] ResponseHeaders { get; set; }

        public int ResponseStatusCode { get; set; }

        public string ResponseStatusDescription { get; set; }

        public int ResponseBodyLength { get; set; }

        public bool ResponseBodyIsText { get; set; }

        public bool ResponseBodyIsImage { get; set; }

        public string ResponseBody { get; set; }

        public string ResponseBodyUrl { get; set; }

    }

}