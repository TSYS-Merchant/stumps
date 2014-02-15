namespace Stumps.Http
{

    using System;
    using System.IO;
    using System.Net;
    using Stumps.Utility;

    internal sealed class StumpsHttpResponse : IStumpsHttpResponse
    {

        private readonly HttpListenerResponse _response;
        private bool _disposed;
        private MemoryStream _responseStream;
        private int _statusCode;

        public StumpsHttpResponse(HttpListenerResponse response)
        {
            _response = response;
            _responseStream = new MemoryStream();

            _statusCode = HttpStatusCodes.HttpOk;
            this.StatusDescription = HttpStatusCodes.GetStatusDescription(_statusCode);
        }

        public HttpListenerResponse ListenerResponse
        {
            get { return _response; }
        }

        public string ContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }

        public WebHeaderCollection Headers
        {
            get { return _response.Headers; }
        }

        public Stream OutputStream
        {
            get { return _responseStream; }
        }

        public bool SendChunked { get; set; }

        public int StatusCode
        {
            get { return _statusCode; }
            set { _statusCode = value; }
        }

        public string StatusDescription { get; set; }

        public void AddHeader(string name, string value)
        {
            _response.AddHeader(name, value);
        }

        public void ClearOutputStream()
        {
            _responseStream.Close();
            _responseStream = new MemoryStream();
        }

        public void FlushResponse()
        {

            // Dump out the response output stream
            StreamUtility.CopyStream(_responseStream, _response.OutputStream, 0);

            // Complete and close the request
            _responseStream.Close();
            _response.OutputStream.Close();
            _response.Close();

        }

        public void Redirect(string url)
        {
            _response.Redirect(url);
        }

        #region IDisposable Members

        public void Dispose()
        {

            if (_responseStream != null && !_disposed)
            {
                _responseStream.Dispose();
                _responseStream = null;
            }

            _disposed = true;

            GC.SuppressFinalize(this);
        }

        #endregion
    }

}