namespace Stumps.Http {

    using System;

    public interface IStumpsHttpContext : IDisposable {

        IStumpsHttpRequest Request { get; }

        IStumpsHttpResponse Response { get; }

    }

}
