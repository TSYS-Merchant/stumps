namespace Stumps
{

    using System;

    public class MockHttpContext : IStumpsHttpContext
    {

        public MockHttpContext()
        {
            this.Request = new MockHttpRequest();
            this.Response = new MockHttpResponse();
        }

        public IStumpsHttpRequest Request { get; set; }

        public IStumpsHttpResponse Response { get; set; }

        public DateTime ReceivedDate
        {
            get { return DateTime.Now; }
        }

        public Guid UniqueIdentifier
        {
            get { return Guid.NewGuid(); }
        }

    }

}