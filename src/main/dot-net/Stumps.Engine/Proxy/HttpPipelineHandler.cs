namespace Stumps.Proxy {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Stumps.Http;

    internal class HttpPipelineHandler : IHttpHandler {

        private readonly List<IHttpHandler> _handlers;

        public HttpPipelineHandler() {
            _handlers = new List<IHttpHandler>();
        }

        public int Count {
            get { return _handlers.Count; }
        }

        public IHttpHandler this[int index] {
            get { return _handlers[index]; }
        }

        public void Add(IHttpHandler handler) {
            _handlers.Add(handler);
        }

        //public IEnumerator<IHttpHandler> GetEnumerator() {
        //    return (IEnumerator<IHttpHandler>) _handlers.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator() {
        //    return (IEnumerator) _handlers.GetEnumerator();
        //}

        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context) {

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            var result = ProcessHandlerResult.Continue;

            foreach ( var handler in _handlers ) {

                result = handler.ProcessRequest(context);

                if ( result == ProcessHandlerResult.Terminate ) {
                    break;
                }

            }

            return result;

        }

    }

}
