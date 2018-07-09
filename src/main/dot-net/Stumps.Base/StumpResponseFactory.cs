namespace Stumps
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class StumpResponseFactory : IStumpResponseFactory
    {
        private const string StumpsHeaderName = "x-stumps";
        private const string ResetHeaderValue = "reset-sequence";

        private readonly Random _random;

        private readonly List<IStumpsHttpResponse> _responses;
        private IStumpsHttpResponse _failureResponse;

        private volatile int _position;

        private ReaderWriterLockSlim _listLock;
        private ReaderWriterLockSlim _positionLock;

        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpResponseFactory"/> class that does not contain any
        ///     is set to respond with an infinate-loop behavior.
        /// </summary>
        public StumpResponseFactory() : this(ResponseFactoryBehavior.OrderedInfinite)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpResponseFactory"/> class that does not contain
        ///     any responses and will, by default, return an HTTP 501 (Not Implemented) error.
        /// </summary>
        /// <param name="behavior">The The behavior of the <see cref="StumpResponseFactory.CreateResponse(IStumpsHttpRequest)"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        public StumpResponseFactory(ResponseFactoryBehavior behavior)
        {
            _listLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _positionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _position = -1;

            _responses = new List<IStumpsHttpResponse>();

            _random = new Random(Environment.TickCount);

            this.Behavior = behavior;
            _failureResponse = HttpErrorResponses.HttpNotImplemented;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpResponseFactory"/> class that does not contain
        ///     any responses while specifying the default response when there are no other responses left in a sequence, 
        ///     or the instance does not contain any responses.
        /// </summary>
        /// <param name="failureResponse">The <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence.</param>
        /// <exception cref="ArgumentNullException"><paramref name="failureResponse"/> is <c>null</c>.</exception>
        /// <remarks>This will set the behavior to <see cref="ResponseFactoryBehavior.OrderedThenFailure"/>.</remarks>
        public StumpResponseFactory(IStumpsHttpResponse failureResponse) : this(ResponseFactoryBehavior.OrderedThenFailure)
        {
            _failureResponse = failureResponse ?? throw new ArgumentNullException(nameof(failureResponse));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpResponseFactory"/> class that contains
        ///     the responses copied from the specified collection.
        /// </summary>
        /// <param name="behavior">The The behavior of the <see cref="StumpResponseFactory.CreateResponse(IStumpsHttpRequest)"/> method when retrieving the next <see cref="IStumpsHttpResponse"/>.</param>
        /// <param name="responses">The collection of <see cref="IStumpsHttpResponse"/> whose elements are copied to the new instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="responses"/> is <c>null</c>.</exception>
        public StumpResponseFactory(ResponseFactoryBehavior behavior, IEnumerable<IStumpsHttpResponse> responses) : this(behavior)
        {
            responses = responses ?? throw new ArgumentNullException(nameof(responses));
            _responses.AddRange(responses);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpResponseFactory"/> class that contains
        ///     the responses copied from the specified collection while specifying the default response when there 
        ///     are no other responses left in a sequence, or the instance does not contain any responses.
        /// </summary>
        /// <param name="failureResponse">The <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence.</param>
        /// <param name="responses">The collection of <see cref="IStumpsHttpResponse"/> whose elements are copied to the new instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="responses"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="defaultResponse"/> is <c>null</c>.</exception>
        public StumpResponseFactory(IStumpsHttpResponse failureResponse, IEnumerable<IStumpsHttpResponse> responses) : this(failureResponse)
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
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
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
        ///     Gets the The behavior of the <see cref="CreateResponse(IStumpsHttpRequest)" /> method when retrieving the next <see cref="IStumpsHttpResponse" />.
        /// </summary>
        /// <value>
        ///     The behavior of the <see cref="CreateResponse(IStumpsHttpRequest)" /> method when retrieving the next <see cref="IStumpsHttpResponse" />.
        /// </value>
        public ResponseFactoryBehavior Behavior
        {
            get;
        }

        /// <summary>
        ///     Gets the number of responses contained within the <see cref="IStumpResponseFactory" />.
        /// </summary>
        /// <value>
        ///     The number of responses contained within the <see cref="IStumpResponseFactory" />.
        /// </value>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
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
        /// Gets the default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance does not contain any responses.
        /// </summary>
        /// <value>
        /// The default <see cref="IStumpsHttpResponse"/> returned when there are no responses left in a sequence, or the instance does not contain any responses.
        /// </value>
        public IStumpsHttpResponse FailureResponse
        {
            get
            {
                _listLock.EnterReadLock();

                var response = _failureResponse;

                _listLock.ExitReadLock();

                return response;
            }
            set
            {
                _listLock.EnterWriteLock();

                _failureResponse = value;

                _listLock.ExitWriteLock();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance has a valid response available.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has a valid response available; otherwise, <c>false</c>.
        /// </value>
        public bool HasResponse
        {
            get
            {
                if (this.Count == 0)
                {
                    return false;
                }

                if (this.Behavior == ResponseFactoryBehavior.OrderedThenFailure &&
                    this.FailureResponse == null)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        ///     Adds the specified response to the <see cref="IStumpResponseFactory" />.
        /// </summary>
        /// <param name="response">The <see cref="T:Stumps.IStumpsHttpResponse" /> to add to the <see cref="IStumpResponseFactory" />.</param>
        /// <returns>The <see cref="IStumpsHttpResponse"/> added to the object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="response" /> is <c>null</c>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory" /> object has been disposed.</exception>
        public IStumpsHttpResponse Add(IStumpsHttpResponse response)
        {
            ThrowExceptionIfDisposed();

            response = response ?? throw new ArgumentNullException(nameof(response));

            _listLock.EnterWriteLock();
            _positionLock.EnterWriteLock();

            _responses.Add(response);
            Interlocked.Exchange(ref _position, -1);

            _listLock.ExitWriteLock();
            _positionLock.ExitWriteLock();

            return response;
        }

        /// <summary>
        ///     Removes all items from the <see cref="IStumpResponseFactory"/>.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
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
        /// <param name="request">The incoming <see cref="IStumpsHttpRequest" />.</param>
        /// <returns>
        ///     An <see cref="T:Stumps.IStumpsHttpResponse" /> object based on an incoming <see cref="T:Stumps.IStumpsHttpRequest" />.
        /// </returns>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
        public virtual IStumpsHttpResponse CreateResponse(IStumpsHttpRequest request)
        {
            ThrowExceptionIfDisposed();

            if (request?.Headers?[StumpsHeaderName]?.Equals(ResetHeaderValue, StringComparison.OrdinalIgnoreCase) == true)
            {
                this.ResetToBeginning();
            }

            if (this.Behavior == ResponseFactoryBehavior.Random)
            {
                return ChooseRandomResponse();
            }
            else if (this.Behavior == ResponseFactoryBehavior.OrderedInfinite)
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
        ///     Removes the <see cref="IStumpResponseFactory"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="IStumpResponseFactory"/>.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
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
            if (this.Behavior == ResponseFactoryBehavior.Random)
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

                return this.FailureResponse;
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
            var response = this.FailureResponse;

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

                return this.FailureResponse;
            }

            var index = _random.Next(_responses.Count);
            var response = _responses[index];

            _listLock.ExitReadLock();

            return response;
        }

        /// <summary>
        /// Throws an exception if the object is disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="StumpResponseFactory"/> object has been disposed.</exception>
        private void ThrowExceptionIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(StumpResponseFactory));
            }
        }
    }
}
