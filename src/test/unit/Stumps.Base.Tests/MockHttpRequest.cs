namespace Stumps
{

    using System;
    using Stumps.Http;

    public class MockHttpRequest : IStumpsHttpRequest
    {

        private byte[] _body;

        public int BodyLength
        {
            get;
            set;
        }

        public IHeaderDictionary Headers
        {
            get;
            set;
        }

        public string HttpMethod
        {
            get;
            set;
        }

        public System.Net.IPEndPoint LocalEndPoint
        {
            get;
            set;
        }

        public string ProtocolVersion
        {
            get;
            set;
        }

        public string RawUrl
        {
            get;
            set;
        }

        public System.Net.IPEndPoint RemoteEndPoint
        {
            get;
            set;
        }

        public byte[] GetBody()
        {
            return _body;
        }

        public void SetBody(byte[] bytes)
        {
            _body = bytes;
        }

    }

}