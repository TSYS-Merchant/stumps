namespace Stumps.Web.Models
{

    using System;

    public class RecordingModel
    {

        public int Index { get; set; }

        public DateTime Date { get; set; }

        public string Method { get; set; }

        public string RawUrl { get; set; }

        public int RequestSize { get; set; }

        public int ResponseSize { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

    }

}