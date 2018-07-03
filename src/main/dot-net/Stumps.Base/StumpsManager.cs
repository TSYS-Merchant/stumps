namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///     A class that manages a collection of <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    internal class StumpsManager : IStumpsManager
    {
        private readonly List<Stump> _stumpList;
        private readonly Dictionary<string, Stump> _stumpReference;

        private ReaderWriterLockSlim _lock;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.StumpsManager"/> class.
        /// </summary>
        public StumpsManager()
        {
            _stumpList = new List<Stump>();
            _stumpReference = new Dictionary<string, Stump>(StringComparer.OrdinalIgnoreCase);
            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.StumpsManager"/> class.
        /// </summary>
        ~StumpsManager() => Dispose(false);

        /// <summary>
        /// Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        /// The count of Stumps in the collection.
        /// </value>
        public int Count
        {
            get => _stumpList.Count;
        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump" /> to the collection.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump" /> to add to the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A <see cref="T:Stumps.Stump" /> with the same identifier already exists.</exception>
        public void AddStump(Stump stump)
        {
            if (stump == null)
            {
                throw new ArgumentNullException(nameof(stump));
            }

            if (_stumpReference.ContainsKey(stump.StumpId))
            {
                throw new ArgumentException(BaseResources.StumpAlreadyExistsError, "stump");
            }

            _lock.EnterWriteLock();
            
            _stumpList.Add(stump);
            _stumpReference.Add(stump.StumpId, stump);

            _lock.ExitWriteLock();
        }

        /// <summary>
        ///     Deletes the specified stump from the collection.
        /// </summary>
        /// <param name="stumpId">The  unique identifier for the stump to remove.</param>
        public void DeleteStump(string stumpId)
        {
            _lock.EnterWriteLock();

            if (_stumpReference.ContainsKey(stumpId))
            {
                var stump = _stumpReference[stumpId];
                _stumpReference.Remove(stumpId);
                _stumpList.Remove(stump);
            }

            _lock.ExitWriteLock();
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> with the specified <paramref name="stumpId"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stump is not found.
        /// </remarks>
        public Stump FindStump(string stumpId)
        {
            _lock.EnterReadLock();

            var stump = _stumpReference[stumpId];
            _lock.ExitReadLock();

            return stump;
        }

        /// <summary>
        ///     Finds the Stump that matches an incoming HTTP request.
        /// </summary>
        /// <param name="context">The incoming HTTP request context.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> that matches the incoming HTTP request.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a matching Stump is not found.
        /// </remarks>
        public Stump FindStumpForContext(IStumpsHttpContext context)
        {
            Stump foundStump = null;

            _lock.EnterReadLock();

            foreach (var stump in _stumpList)
            {
                if (stump.IsMatch(context))
                {
                    foundStump = stump;
                    break;
                }
            }

            _lock.ExitReadLock();

            return foundStump;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            _disposed = true;

            if (_lock != null)
            {
                _lock.Dispose();
                _lock = null;
            }
        }
    }
}
