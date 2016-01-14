namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using Stumps.Http;
    using System.Threading.Tasks;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that executes multiple child
    ///     <see cref="T:Stumps.Http.IHttpHandler"/> instances.
    /// </summary>
    internal class HttpPipelineHandler : IHttpHandler
    {

        private readonly List<IHttpHandler> _handlers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.HttpPipelineHandler"/> class.
        /// </summary>
        public HttpPipelineHandler()
        {
            _handlers = new List<IHttpHandler>();
        }

        /// <summary>
        ///     Occurs when an incomming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        /// Gets the number of child <see cref="T:Stumps.Http.IHttpHandler"/> instances.
        /// </summary>
        /// <value>
        /// The number of child <see cref="T:Stumps.Http.IHttpHandler"/> instances.
        /// </value>
        public int Count
        {
            get { return _handlers.Count; }
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.Http.IHttpHandler"/> at the specified index.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.Http.IHttpHandler"/> at the specified index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>An <see cref="T:Stumps.Http.IHttpHandler"/>.</returns>
        public IHttpHandler this[int index]
        {
            get { return _handlers[index]; }
        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        ///     A member of the <see cref="T:Stumps.Http.ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var result = ProcessHandlerResult.Continue;

            foreach (var handler in _handlers)
            {

                result = await handler.ProcessRequest(context);

                if (result != ProcessHandlerResult.Continue)
                {
                    break;
                }

            }

            if (this.ContextProcessed != null)
            {
                this.ContextProcessed(this, new StumpsContextEventArgs(context));
            }

            return result;

        }

        /// <summary>
        /// Adds the specified child <see cref="T:Stumps.Http.IHttpHandler"/> to the end of the pipeline.
        /// </summary>
        /// <param name="handler">The child <see cref="T:Stumps.Http.IHttpHandler"/> to add.</param>
        public void Add(IHttpHandler handler)
        {
            _handlers.Add(handler);
        }

    }

}