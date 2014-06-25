namespace Stumps.Server
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents a collection of recordings from a Stumps server.
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

            var startingIndex = afterIndex == int.MaxValue ? afterIndex - 1 : afterIndex;

            startingIndex++;

            lock (_syncRoot)
            {

                for (var i = startingIndex; i < _recordings.Count; i++)
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
        ///     Adds the specified <see cref="T:Stumps.IStumpsHttpContext"/> to the collection.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext"/> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        internal void Add(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var recordedContext = new RecordedContext(context, ContentDecoderHandling.DecodeRequired);

            lock (_syncRoot)
            {
                _recordings.Add(recordedContext);
            }

        }

    }

}