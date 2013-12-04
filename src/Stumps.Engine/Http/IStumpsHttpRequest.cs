namespace Stumps.Http {

    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;

    public interface IStumpsHttpRequest : IDisposable {

        string ContentType { get; }

        NameValueCollection Headers { get; }

        string HttpMethod { get; }

        Stream InputStream { get; }

        bool IsSecureConnection { get; }

        IPEndPoint LocalEndPoint { get; }

        Version ProtocolVersion { get; }

        NameValueCollection QueryString { get; }

        string RawUrl { get; }

        string Referer { get; }

        IPEndPoint RemoteEndPoint { get; }

        Uri Url { get; }

        string UserAgent { get; }

    }

}
