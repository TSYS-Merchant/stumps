namespace Stumps.Server
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents a collection of recordings from a proxy server.
    /// </summary>
    public sealed class Recordings
    {

        private readonly List<RecordedContext> _recordings;
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Recordings"/> class.
        /// </summary>
        public Recordings()
        {
            _recordings = new List<RecordedContext>();
        }

        /// <summary>
        ///     Gets the count of recordings in the collection.
        /// </summary>
        /// <value>
        ///     The count of recordings in the collection.
        /// </value>
        public int Count
        {
            get { return _recordings.Count; }
        }

        /// <summary>
        ///     Adds the specified <see cref="T:Stumps.IStumpsHttpContext"/> to the collection.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext"/> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public void Add(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var recordedContext = new RecordedContext();
            
            recordedContext.Request.Body = context.Request.GetBody();
            recordedContext.Request.BodyContentType = context.Request.Headers["Content-Type"] ?? string.Empty;
            recordedContext.Request.HttpMethod = context.Request.HttpMethod;
            recordedContext.Request.RawUrl = context.Request.RawUrl;
            CopyHeaders(context.Request.Headers, recordedContext.Request);
            DecodeBody(recordedContext.Request);
            DetermineBodyIsImage(recordedContext.Request);

            if (!recordedContext.Request.BodyIsImage)
            {
                DetermineBodyIsText(recordedContext.Request);
            }

            recordedContext.Response.Body = context.Response.GetBody();
            recordedContext.Response.BodyContentType = context.Response.Headers["Content-Type"] ?? string.Empty;
            recordedContext.Response.StatusCode = context.Response.StatusCode;
            recordedContext.Response.StatusDescription = context.Response.StatusDescription;
            CopyHeaders(context.Response.Headers, recordedContext.Response);
            DecodeBody(recordedContext.Response);
            DetermineBodyIsImage(recordedContext.Response);

            if (!recordedContext.Response.BodyIsImage)
            {
                DetermineBodyIsText(recordedContext.Response);
            }

            lock (_syncRoot)
            {
                _recordings.Add(recordedContext);
            }

        }

        /// <summary>
        ///     Clears all recorded contexts from the instance.
        /// </summary>
        public void Clear()
        {
            lock (_syncRoot)
            {
                _recordings.Clear();
            }
        }

        /// <summary>
        ///     Finds all recordings after the specified index.
        /// </summary>
        /// <param name="afterIndex">The index used to find all recorded contexts after.</param>
        /// <returns>
        ///     A generic list of <see cref="T:Stumps.Server.RecordedContext"/> objects.
        /// </returns>
        public IList<RecordedContext> Find(int afterIndex)
        {

            var returnList = new List<RecordedContext>();

            afterIndex = afterIndex == int.MaxValue ? afterIndex - 1 : afterIndex;

            afterIndex++;

            lock (_syncRoot)
            {

                for (var i = afterIndex; i < _recordings.Count; i++)
                {
                    returnList.Add(_recordings[i]);
                }

            }

            return returnList;

        }

        /// <summary>
        ///     Finds the recorded context at the specified index.
        /// </summary>
        /// <param name="index">The index of the recorded context.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.RecordedContext"/> found at the specified <paramref name="index"/>.
        /// </returns>
        /// <remarks>
        ///     If a recorded context cannot be found at the specified <paramref name="index"/>, a <c>null</c>
        ///     value is returned.
        /// </remarks>
        public RecordedContext FindAt(int index)
        {

            RecordedContext context = null;

            lock (_syncRoot)
            {

                if (index < _recordings.Count)
                {
                    context = _recordings[index];
                }

            }

            return context;

        }

        /// <summary>
        ///     Copies the headers from a <see cref="T:Stumps.IHeaderDictionary"/> to the specified <see cref="T:Stumps.Server.IRecordedContextPart"/>.
        /// </summary>
        /// <param name="headerDictionary">The header dictionary used as the source of the headers.</param>
        /// <param name="contextPart">The recorded context part used as the target for the headers.</param>
        private void CopyHeaders(IHttpHeaders headerDictionary, IRecordedContextPart contextPart)
        {

            foreach (var headerName in headerDictionary.HeaderNames)
            {
                var header = new HttpHeader
                {
                    Name = headerName,
                    Value = headerDictionary[headerName]
                };

                contextPart.Headers.Add(header);
            }

        }

        /// <summary>
        ///     Decodes the body of a based on the content encoding.
        /// </summary>
        /// <param name="part">The <see cref="T:Stumps.Server.IRecordedContextPart"/> part containing the body to decode.</param>
        private void DecodeBody(IRecordedContextPart part)
        {

            var buffer = part.Body;
            var header = part.FindHeader("Content-Encoding");

            if (header != null)
            {
                var encoder = new ContentEncoder(header.Value);
                buffer = encoder.Decode(buffer);
            }

            part.Body = buffer;

        }

        /// <summary>
        ///     Determines if the body of the recorded context part is an image.
        /// </summary>
        /// <param name="part">The part of the recorded context to analyze.</param>
        private void DetermineBodyIsImage(IRecordedContextPart part)
        {

            if (part.BodyContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                part.BodyIsImage = true;
            }

        }

        /// <summary>
        ///     Determines if the body of the recorded context part is text.
        /// </summary>
        /// <param name="part">The part of the recorded context to analyze.</param>
        private void DetermineBodyIsText(IRecordedContextPart part)
        {

            part.BodyIsText = TextAnalyzer.IsText(part.Body);

        }

    }

}