namespace Stumps
{

    using Stumps.Http;

    public class MockHttpResponse : IStumpsHttpResponse
    {

        public MockHttpResponse()
        {
            this.Headers = new System.Net.WebHeaderCollection();
        }

        #region IStumpsHttpResponse Members

        public string ContentType { get; set; }

        public System.Net.WebHeaderCollection Headers { get; set; }

        public System.IO.Stream OutputStream { get; set; }

        public bool SendChunked { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public void AddHeader(string name, string value)
        {
            this.Headers.Add(name, value);
        }

        public void ClearOutputStream()
        {
            this.OutputStream = new System.IO.MemoryStream();
        }

        public void FlushResponse()
        {
        }

        public void Redirect(string url)
        {
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {

            if (this.OutputStream != null)
            {
                this.OutputStream.Dispose();
                this.OutputStream = null;
            }

        }

        #endregion
    }

}