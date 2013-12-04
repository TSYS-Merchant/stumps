namespace Stumps.Proxy {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Stumps.Http;

    internal class HttpPipelineHandler : IHttpHandler, IEnumerable<IHttpHandler> {

        private readonly List<IHttpHandler> _handlers;

        public HttpPipelineHandler() {
            _handlers = new List<IHttpHandler>();
        }

        public IHttpHandler this[int index] {
            get { return _handlers[index]; }
        }

        public int Count {
            get { return _handlers.Count; }
        }

        public void Add(IHttpHandler handler) {
            _handlers.Add(handler);
        }

        public void Clear() {
            _handlers.Clear();
        }

        public bool Contains(IHttpHandler handler) {
            return _handlers.Contains(handler);
        }

        public IEnumerator<IHttpHandler> GetEnumerator() {
            return (IEnumerator<IHttpHandler>) _handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator) _handlers.GetEnumerator();
        }

        public int IndexOf(IHttpHandler handler) {
            return _handlers.IndexOf(handler);
        }

        public void Insert(int index, IHttpHandler handler) {
            _handlers.Insert(index, handler);
        }

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

        public void RemoteAt(int index) {
            _handlers.RemoveAt(index);
        }

    }

}
