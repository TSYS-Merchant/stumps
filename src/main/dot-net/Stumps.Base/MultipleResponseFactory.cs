namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class MultipleResponseFactory : IStumpsHttpResponseFactory, IDisposable
    {
        private const string StumpsHeaderName = "x-stumps";
        private const string ResetHeaderValue = "reset-sequence";

        private readonly Random _random;

        private readonly List<IStumpsHttpResponse> _responses;

        private volatile int _position;

        private ReaderWriterLockSlim _listLock;
        private ReaderWriterLockSlim _positionLock;

        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipleResponseFactory"/> class that does not contain
        ///     any responses and will, by default, return an HTTP 501 (Not Implemented) error.
        /// </summary>
        /// <param name="behavior">The behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        public MultipleResponseFactory(MultipleResponseFactoryBehavior behavior)
        {
            _listLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _positionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _position = -1;

            _responses = new List<IStumpsHttpResponse>();

            _random = new Random(Environment.TickCount);

            this.Behavior = behavior;
            this.DefaultResponse = HttpErrorResponses.HttpNotImplemented;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipleResponseFactory"/> class that does not contain
        ///     any responses while specifying the default response when there are no other responses left in a sequence, 
        ///     or the instance does not contain any responses.
        /// </summary>
        /// <param name="behavior">The behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        /// <param name="defaultResponse">The default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance doe snot contain any responses.</param>
        /// <exception cref="ArgumentNullException"><paramref name="defaultResponse"/> is <c>null</c>.</exception>
        public MultipleResponseFactory(MultipleResponseFactoryBehavior behavior, IStumpsHttpResponse defaultResponse) : this(behavior)
        {
            this.DefaultResponse = defaultResponse ?? throw new ArgumentNullException(nameof(defaultResponse));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipleResponseFactory"/> class that contains
        ///     the responses copied from the specified collection.
        /// </summary>
        /// <param name="behavior">The behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        /// <param name="responses">The collection of <see cref="IStumpsHttpResponse"/> whose elements are copied to the new instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="responses"/> is <c>null</c>.</exception>
        public MultipleResponseFactory(MultipleResponseFactoryBehavior behavior, IEnumerable<IStumpsHttpResponse> responses) : this(behavior)
        {
            responses = responses ?? throw new ArgumentNullException(nameof(responses));
            _responses.AddRange(responses);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MultipleResponseFactory"/> class that contains
        ///     the responses copied from the specified collection while specifying the default response when there 
        ///     are no other responses left in a sequence, or the instance does not contain any responses.
        /// </summary>
        /// <param name="behavior">The behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        /// <param name="responses">The collection of <see cref="IStumpsHttpResponse"/> whose elements are copied to the new instance.</param>
        /// <param name="defaultResponse">The default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance doe snot contain any responses.</param>
        /// <exception cref="ArgumentNullException"><paramref name="responses"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="defaultResponse"/> is <c>null</c>.</exception>
        public MultipleResponseFactory(MultipleResponseFactoryBehavior behavior, IEnumerable<IStumpsHttpResponse> responses, IStumpsHttpResponse defaultResponse) : this(behavior, defaultResponse)
        {
            responses = responses ?? throw new ArgumentNullException(nameof(responses));
            _responses.AddRange(responses);
        }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse"/> at the specified index.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse"/> at the specified index.
        /// </value>
        /// <param name="index">The zero-based index of the <see cref="T:Stumps.IStumpsHttpResponse"/> to get.</param>
        /// <returns>The <see cref="Stumps.IStumpsHttpResponse"/> at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><c>index</c> is not a valid index in the <see cref="T:Stumps.StumpsHttpResponseFactory"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public IStumpsHttpResponse this[int index]
        {
            get
            {
                ThrowExceptionIfDisposed();

                try
                {
                    _listLock.EnterReadLock();
                    return _responses[index];
                }
                catch
                {
                    throw;
                }
                finally
                {
                    _listLock.ExitReadLock();
                }
            }
        }

        /// <summary>
        ///     Gets the behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.
        /// </summary>
        /// <value>
        ///     The behavior of the <see cref="MultipleResponseFactory.Behavior"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.
        /// </value>
        public MultipleResponseFactoryBehavior Behavior
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets the number of responses contained within the <see cref="MultipleResponseFactory"/>.
        /// </summary>
        /// <value>
        ///     The number of responses contained within the <see cref="MultipleHttpResponseFactory"/>.
        /// </value>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public int Count
        {
            get
            {
                ThrowExceptionIfDisposed();

                _listLock.EnterReadLock();

                var count = _responses.Count;

                _listLock.ExitReadLock();

                return count;
            }
        }

        /// <summary>
        /// Gets the default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance doe snot contain any responses.
        /// </summary>
        /// <value>
        /// The default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance doe snot contain any responses.
        /// </value>
        public IStumpsHttpResponse DefaultResponse
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has a valid response available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has a valid response available; otherwise, <c>false</c>.
        /// </value>
        public bool HasResponse => true;

        /// <summary>
        ///     Adds the specified response to the <see cref="T:Stumps.StumpsHttpResponseFactory"/>.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.IStumpsHttpResponse"/> to add to the <see cref="T:Stumps.StumpsHttpResponseFactory"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="response"/> is <c>null</c>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public void Add(IStumpsHttpResponse response)
        {
            ThrowExceptionIfDisposed();

            response = response ?? throw new ArgumentNullException(nameof(response));

            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            _responses.Add(response);
            Interlocked.Exchange(ref _position, -1);

            _listLock.ExitWriteLock();
            _positionLock.ExitWriteLock();
        }

        /// <summary>
        ///     Removes all items from the <see cref="T:Stumps.StumpsHttpResponseFactory"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public void Clear()
        {
            ThrowExceptionIfDisposed();

            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            _responses.Clear();
            Interlocked.Exchange(ref _position, -1);

            _listLock.ExitWriteLock();
            _positionLock.ExitWriteLock();
        }

        /// <summary>
        ///     Creates an <see cref="T:Stumps.IStumpsHttpResponse" /> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest" />
        ///     which is returned for a Stump positivly matching all necessary criteria.
        /// </summary>
        /// <param name="request">The incoming <see cref="T:Stumps.IStumpsHttpRequest" />.</param>
        /// <returns>
        ///     An <see cref="T:Stumps.IStumpsHttpResponse" /> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request)
        {
            ThrowExceptionIfDisposed();

            if (request?.Headers?[StumpsHeaderName]?.Equals(ResetHeaderValue, StringComparison.OrdinalIgnoreCase) == true)
            {
                this.ResetToBeginning();
            }

            if (this.Behavior == MultipleResponseFactoryBehavior.Random)
            {
                return ChooseRandomResponse();
            }
            else if (this.Behavior == MultipleResponseFactoryBehavior.OrderedInfinite)
            {
                return ChooseNextResponse();
            }

            return ChooseNextResponseOrDefault();
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
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            _disposed = true;

            if (_listLock != null)
            {
                _listLock.Dispose();
                _listLock = null;
            }

            if (_positionLock != null)
            {
                _positionLock.Dispose();
                _positionLock = null;
            }
        }

        /// <summary>
        ///     Removes the <see cref="T:Stumps.StumpsHttpResponseFactory"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="T:Stumps.StumpsHttpResponseFactory"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        public void RemoveAt(int index)
        {
            ThrowExceptionIfDisposed();

            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            try
            {
                _responses.RemoveAt(index);
                Interlocked.Exchange(ref _position, -1);
            }
            catch
            {
                throw;
            }
            finally
            {
                _listLock.ExitWriteLock();
                _positionLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Resets the next request to the start of the sequence regardless of the current position.
        /// </summary>
        public void ResetToBeginning()
        {
            if (this.Behavior == MultipleResponseFactoryBehavior.Random)
            {
                return;
            }

            _positionLock.EnterWriteLock();

            Interlocked.Exchange(ref _position, -1);

            _positionLock.ExitWriteLock();
        }

        /// <summary>
        ///     Chooses the next <see cref="IStumpsHttpResponse"/> in the list of available responses after the response returned from a previous request.
        /// </summary>
        /// <returns>A <see cref="IStumpsHttpResponse"/> that represents the next available response after the response from a previous request.</returns>
        /// <remarks>If a response is not available because the list of responses has been exceeded, the first response is returned.</remarks>
        private IStumpsHttpResponse ChooseNextResponse()
        {
            _positionLock.EnterWriteLock();
            _listLock.EnterReadLock();

            if (_responses.Count == 0)
            {
                _positionLock.ExitWriteLock();
                _listLock.ExitReadLock();

                return this.DefaultResponse;
            }

            var index = Interlocked.Increment(ref _position);

            if (_position >= _responses.Count)
            {
                Interlocked.Exchange(ref _position, 0);
                index = 0;
            }

            _positionLock.ExitWriteLock();

            var response = _responses[index];

            _listLock.ExitReadLock();

            return response;
        }

        /// <summary>
        ///     Chooses the next <see cref="IStumpsHttpResponse"/> in the list of available responses after the response returned from a previous request.
        /// </summary>
        /// <returns>A <see cref="IStumpsHttpResponse"/> that represents the next available response after the response from a previous request.</returns>
        /// <remarks>If a response is not available because the list of responses has been exceeded, the default response is returned.</remarks>
        private IStumpsHttpResponse ChooseNextResponseOrDefault()
        {
            var response = this.DefaultResponse;

            _listLock.EnterReadLock();

            var index = Interlocked.Increment(ref _position);

            if (_position < _responses.Count)
            {
                response = _responses[_position];
            }

            _listLock.ExitReadLock();

            return response;
        }

        /// <summary>
        ///     Chooses a <see cref="IStumpsHttpResponse"/> at random from the list of available responses.
        /// </summary>
        /// <returns>A <see cref="IStumpsHttpResponse"/> chosen at random from the list of available responses.</returns>
        private IStumpsHttpResponse ChooseRandomResponse()
        {
            _listLock.EnterReadLock();

            if (_responses.Count == 0)
            {
                _listLock.ExitReadLock();

                return this.DefaultResponse;
            }

            var index = _random.Next(_responses.Count);
            var response = _responses[index];

            _listLock.ExitReadLock();

            return response;
        }

        /// <summary>
        /// Throws an exception if the object is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="MultipleResponseFactory"/> object has been disposed.</exception>
        private void ThrowExceptionIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MultipleResponseFactory));
            }
        }
    }
}
