namespace Stumps.Proxy {

    using System.Collections.Generic;

    internal interface IRecordedContextPart {

        byte[] Body { get; set; }

        string BodyContentType { get; set; }

        bool BodyIsImage { get; set; }

        bool BodyIsText { get; set; }

        IList<HttpHeader> Headers { get; set; }

        HttpHeader FindHeader(string name);

    }

}
