namespace Stumps.Http
{

    using System;

    internal sealed class StumpsContextEventArgs : EventArgs
    {

        public StumpsContextEventArgs(StumpsHttpContext context)
        {
            this.Context = context;
        }

        public StumpsHttpContext Context { get; private set; }

    }

}