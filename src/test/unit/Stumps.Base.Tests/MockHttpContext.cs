namespace Stumps
{
    using System;

    public class MockHttpContext : IStumpsHttpContext
    {
        public MockHttpContext() : this(null, null)
        {
        }

        public MockHttpContext(IStumpsHttpRequest request, IStumpsHttpResponse response)
        {
            this.Request = request ?? new MockHttpRequest();
            this.Response = response ?? new MockHttpResponse();
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
