namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Stumps.Http;

    internal class MockHandler : IHttpHandler
    {

        private readonly List<Tuple<string, string>> _headers;
        private byte[] _body;

        public MockHandler()
        {

            _headers = new List<Tuple<string, string>>();

            this.ContentType = "text/plain";
            this.StatusCode = 200;
            this.StatusDescription = "OK";
            _body = null;

        }

        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        public string ContentType { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public int HandlerId { get; set; }

        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context)
        {

            foreach (var value in _headers)
            {
                context.Response.Headers[value.Item1] = value.Item2;
            }

            context.Response.StatusCode = this.StatusCode;
            context.Response.StatusDescription = this.StatusDescription;

            if (_body != null && _body.Length > 0)
            {
                context.Response.ClearBody();
                context.Response.AppendToBody(_body);
            }

            if (this.ContextProcessed != null)
            {
                this.ContextProcessed(this, new StumpsContextEventArgs(context));
            }

            return ProcessHandlerResult.Terminate;

        }

        public void AddHeader(string headerName, string headerValue)
        {

            _headers.Add(new Tuple<string, string>(headerName, headerValue));

        }

        public void UpdateBody(string value)
        {

            _body = Encoding.UTF8.GetBytes(value);

        }

    }

}