namespace Stumps.Server.Proxy
{

    using System;
    using System.Collections.Generic;
    using Stumps.Utility;

    /// <summary>
    ///     A class that represents a collection of recordings from a proxy server.
    /// </summary>
    public sealed class ProxyRecordings
    {

        private readonly List<RecordedContext> _recordings;
        private readonly object _syncRoot = new object();

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Proxy.ProxyRecordings"/> class.
        /// </summary>
        public ProxyRecordings()
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
        ///     Adds the specified <see cref="T:Stumps.Server.Proxy.RecordedContext"/> to the collection.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Server.Proxy.RecordedContext"/> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public void Add(RecordedContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            DetermineContentType(context.Request);
            DetermineBodyIsImage(context.Request);

            if (!context.Request.BodyIsImage)
            {
                DetermineBodyIsText(context.Request);
            }

            DetermineContentType(context.Response);
            DetermineBodyIsImage(context.Response);

            if (!context.Response.BodyIsImage)
            {
                DetermineBodyIsText(context.Response);
            }

            lock (_syncRoot)
            {
                _recordings.Add(context);
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
        ///     A generic list of <see cref="T:Stumps.Server.Proxy.RecordedContext"/> objects.
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
        ///     A <see cref="T:Stumps.Server.Proxy.RecordedContext"/> found at the specified <paramref name="index"/>.
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
        ///     Determines the content type of the recorded context part.
        /// </summary>
        /// <param name="part">The part of the recorded context to analyze.</param>
        private void DetermineContentType(IRecordedContextPart part)
        {

            var header = part.FindHeader("content-type");
            header = header ?? new HttpHeader
            {
                Name = string.Empty,
                Value = string.Empty
            };
            part.BodyContentType = header.Value;

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