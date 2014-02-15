namespace Stumps.Http
{

    using System;
    using System.IO;
    using System.Net;

    public interface IStumpsHttpResponse : IDisposable
    {

        string ContentType { get; set; }

        WebHeaderCollection Headers { get; }

        Stream OutputStream { get; }

        bool SendChunked { get; set; }

        int StatusCode { get; set; }

        string StatusDescription { get; set; }

        void AddHeader(string name, string value);

        void ClearOutputStream();

        void FlushResponse();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
            MessageId = "0#", Justification = "Based on the underlying framework")]
        void Redirect(string url);

    }

}