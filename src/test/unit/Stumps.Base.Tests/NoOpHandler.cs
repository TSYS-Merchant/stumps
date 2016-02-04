namespace Stumps
{

    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Stumps.Http;


    internal class NoOpHandler : IHttpHandler
    {

        private ProcessHandlerResult _cannedResponse;
        private int _processRequestCalls;

        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        public NoOpHandler(ProcessHandlerResult cannedResponse)
        {
            _cannedResponse = cannedResponse;
        }

        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            Interlocked.Increment(ref _processRequestCalls);

            var contextEvent = this.ContextProcessed;
            if (contextEvent != null)
            {
                contextEvent(this, new StumpsContextEventArgs(context));
            }

            return _cannedResponse;
        }

        public int ProcessRequestCalls()
        {
            return _processRequestCalls;
        }

    }

}
