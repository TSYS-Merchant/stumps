namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="IHttpHandler"/> interface that executes multiple child
    ///     <see cref="IHttpHandler"/> instances.
    /// </summary>
    internal class HttpPipelineHandler : IHttpHandler
    {
        private readonly List<IHttpHandler> _handlers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpPipelineHandler"/> class.
        /// </summary>
        public HttpPipelineHandler()
        {
            _handlers = new List<IHttpHandler>();
        }

        /// <summary>
        ///     Occurs when an incoming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        /// Gets the number of child <see cref="IHttpHandler"/> instances.
        /// </summary>
        /// <value>
        /// The number of child <see cref="IHttpHandler"/> instances.
        /// </value>
        public int Count
        {
            get => _handlers.Count;
        }

        /// <summary>
        ///     Gets the <see cref="IHttpHandler"/> at the specified index.
        /// </summary>
        /// <value>
        ///     The <see cref="IHttpHandler"/> at the specified index.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>An <see cref="IHttpHandler"/>.</returns>
        public IHttpHandler this[int index]
        {
            get => _handlers[index];
        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        ///     A member of the <see cref="ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            var result = ProcessHandlerResult.Continue;

            foreach (var handler in _handlers)
            {
                result = await handler.ProcessRequest(context);

                if (result != ProcessHandlerResult.Continue)
                {
                    break;
                }
            }

            this.ContextProcessed?.Invoke(this, new StumpsContextEventArgs(context));

            return result;
        }

        /// <summary>
        /// Adds the specified child <see cref="IHttpHandler"/> to the end of the pipeline.
        /// </summary>
        /// <param name="handler">The child <see cref="IHttpHandler"/> to add.</param>
        public void Add(IHttpHandler handler) => _handlers.Add(handler);
    }
}
