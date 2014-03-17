namespace Stumps
{

    using System;
    using Stumps.Http;

    public class MockHttpResponse : IStumpsHttpResponse
    {

        private byte[] _bodyBuffer;
        
        public MockHttpResponse()
        {
            _bodyBuffer = new byte[0];
            this.Headers = new HeaderDictionary();
        }

        public int BodyLength
        {
            get { return _bodyBuffer.Length; }
        }

        public IHeaderDictionary Headers
        {
            get;
            private set;
        }

        public string RedirectAddress
        {
            get; set;
        }

        public int StatusCode
        {
            get; set;
        }

        public string StatusDescription
        {
            get; set;
        }

        public void AppendToBody(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            var newBodyLength = _bodyBuffer.Length + bytes.Length;
            var newBuffer = new byte[newBodyLength];

            Buffer.BlockCopy(_bodyBuffer, 0, newBuffer, 0, _bodyBuffer.Length);
            Buffer.BlockCopy(bytes, 0, newBuffer, _bodyBuffer.Length, bytes.Length);

            _bodyBuffer = newBuffer;
        }

        public void ClearBody()
        {
            _bodyBuffer = new byte[0];
        }

        public byte[] GetBody()
        {
            return _bodyBuffer;
        }

    }

}