namespace Stumps
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Stumps.Http;

    internal class NoOpHandler : IHttpHandler
    {
        private readonly ProcessHandlerResult _cannedResponse;
        private int _processRequestCalls;

        public NoOpHandler(ProcessHandlerResult cannedResponse)
        {
            _cannedResponse = cannedResponse;
        }

        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            Interlocked.Increment(ref _processRequestCalls);

            this.ContextProcessed?.Invoke(this, new StumpsContextEventArgs(context));

            return await Task.FromResult(_cannedResponse);
        }

        public int ProcessRequestCalls()
        {
            return _processRequestCalls;
        }
    }
}
