namespace Stumps.Web.Models
{

    public class StumpModel
    {

        public string Name { get; set; }

        public StumpOrigin Origin { get; set; }

        public int RecordId { get; set; }

        public HeaderModel[] RequestHeaderMatch { get; set; }

        public string RequestBody { get; set; }

        public BodyMatch RequestBodyMatch { get; set; }

        public bool RequestBodyIsImage { get; set; }

        public bool RequestBodyIsText { get; set; }

        public int RequestBodyLength { get; set; }

        public string[] RequestBodyMatchValues { get; set; }

        public string RequestBodyUrl { get; set; }

        public string RequestHttpMethod { get; set; }

        public bool RequestHttpMethodMatch { get; set; }

        public string RequestUrl { get; set; }

        public bool RequestUrlMatch { get; set; }

        public string ResponseBody { get; set; }

        public bool ResponseBodyIsImage { get; set; }

        public bool ResponseBodyIsText { get; set; }

        public int ResponseBodyLength { get; set; }

        public string ResponseBodyModification { get; set; }

        public BodySource ResponseBodySource { get; set; }

        public string ResponseBodyUrl { get; set; }

        public HeaderModel[] ResponseHeaders { get; set; }

        public int ResponseStatusCode { get; set; }

        public string ResponseStatusDescription { get; set; }

        public string StumpId { get; set; }

    }

}