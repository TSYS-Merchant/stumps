namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using Stumps.Utility;

    public sealed class ProxyRecordings {

        private readonly List<RecordedContext> _recordings;
        private readonly object _syncRoot = new object();

        public ProxyRecordings() {
            _recordings = new List<RecordedContext>();
        }

        public int Count { get { return _recordings.Count; } }

        public void Add(RecordedContext context) {

            if ( context == null ) {
                throw new ArgumentNullException("context");
            }

            determineContentType(context.Request);
            determineBodyIsImage(context.Request);

            if ( !context.Request.BodyIsImage ) {
                determineBodyIsText(context.Request);
            }

            determineContentType(context.Response);
            determineBodyIsImage(context.Response);

            if ( !context.Response.BodyIsImage ) {
                determineBodyIsText(context.Response);
            }

            lock ( _syncRoot ) {
                _recordings.Add(context);
            }
        }

        public void Clear() {
            lock ( _syncRoot ) {
                _recordings.Clear();
            }
        }

        public IList<RecordedContext> Find(int afterIndex) {

            var returnList = new List<RecordedContext>();

            afterIndex = (afterIndex == int.MaxValue ? afterIndex - 1 : afterIndex);

            afterIndex++;

            lock ( _syncRoot ) {

                for ( var i = afterIndex; i < _recordings.Count; i++ ) {
                    returnList.Add(_recordings[i]);
                }

            }

            return returnList;

        }

        public RecordedContext FindAt(int index) {

            RecordedContext context = null;

            lock ( _syncRoot ) {

                if ( index < _recordings.Count ) {
                    context = _recordings[index];
                }

            }

            return context;

        }

        private void determineContentType(IRecordedContextPart part) {

            var header = part.FindHeader("content-type");
            header = header ?? new HttpHeader() { Name = string.Empty, Value = string.Empty };
            part.BodyContentType = header.Value;

        }

        private void determineBodyIsText(IRecordedContextPart part) {

            part.BodyIsText = StringUtility.IsText(part.Body);

        }

        private void determineBodyIsImage(IRecordedContextPart part) {

            if ( part.BodyContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase) ) {
                part.BodyIsImage = true;
            }

        }

    }

}
