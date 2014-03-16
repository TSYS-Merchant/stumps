namespace Stumps.Server.Proxy
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Stumps.Server.Data;
    using Stumps.Server.Utility;
    using Stumps.Rules;

    /// <summary>
    ///     A class that represents a collection of Stumps.
    /// </summary>
    public class ProxyStumps : IDisposable
    {

        private readonly IDataAccess _dataAccess;
        private readonly string _proxyId;
        private readonly List<Stump> _stumpList;
        private readonly Dictionary<string, Stump> _stumpReference;
        private bool _disposed;
        private ReaderWriterLockSlim _lock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.Proxy.ProxyStumps"/> class.
        /// </summary>
        /// <param name="proxyId">The unique identifier for the proxy.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        public ProxyStumps(string proxyId, IDataAccess dataAccess)
        {

            _stumpList = new List<Stump>();
            _stumpReference = new Dictionary<string, Stump>(StringComparer.OrdinalIgnoreCase);

            _dataAccess = dataAccess;
            _proxyId = proxyId;

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        }

        /// <summary>
        /// Finalizes an instance of the <see cref="T:Stumps.Server.Proxy.ProxyStumps"/> class.
        /// </summary>
        ~ProxyStumps()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        /// The count of Stumps in the collection.
        /// </value>
        public int Count
        {
            get { return _stumpList.Count; }
        }

        /// <summary>
        ///     Creates a new Stump.
        /// </summary>
        /// <param name="contract">The contract used to create the Stump.</param>
        /// <returns>
        ///     An updated <see cref="T:Stumps.Server.Proxy.StumpContract"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="contract"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">A stump with the same name already exists.</exception>
        public StumpContract CreateStump(StumpContract contract)
        {

            if (contract == null)
            {
                throw new ArgumentNullException("contract");
            }

            if (string.IsNullOrEmpty(contract.StumpId))
            {
                contract.StumpId = RandomGenerator.GenerateIdentifier();
            }

            if (this.StumpNameExists(contract.StumpName))
            {
                throw new ArgumentException(Resources.StumpNameUsedError);
            }

            var entity = CreateEntityFromContract(contract);

            _dataAccess.StumpCreate(_proxyId, entity, contract.MatchBody, contract.Response.Body);

            UnwrapAndAddStump(contract);

            return contract;

        }

        /// <summary>
        ///     Deletes an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        public void DeleteStump(string stumpId)
        {

            _lock.EnterWriteLock();

            var stump = _stumpReference[stumpId];
            _stumpReference.Remove(stumpId);
            _stumpList.Remove(stump);

            _dataAccess.StumpDelete(_proxyId, stumpId);

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
        ///     Loads the Stumps from the data access provider.
        /// </summary>
        public void Load()
        {

            var entities = _dataAccess.StumpFindAll(_proxyId);

            foreach (var entity in entities)
            {
                var contract = CreateContractFromEntity(entity);
                UnwrapAndAddStump(contract);
            }

        }

        /// <summary>
        ///     Finds a list of all Stump contracts.
        /// </summary>
        /// <returns>
        ///     A generic list of all <see cref="T:Stumps.Server.Proxy.StumpContract"/> objects.
        /// </returns>
        public IList<StumpContract> FindAllContracts()
        {

            var stumpContractList = new List<StumpContract>();

            _lock.EnterReadLock();

            foreach (var stump in _stumpList)
            {
                stumpContractList.Add(stump.Contract);
            }

            _lock.ExitReadLock();

            return stumpContractList;

        }

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.Stump"/> with the specified <paramref name="stumpId"/>.
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
        ///     Finds the Stump that matches an incomming HTTP request.
        /// </summary>
        /// <param name="context">The incoming HTTP request context.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.Stump"/> that matches the incomming HTTP request.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a matching Stump is not found.
        /// </remarks>
        public Stump FindStump(IStumpsHttpContext context)
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
        /// Determines if a stump with the specified name exists.
        /// </summary>
        /// <param name="stumpName">The name of the stump.</param>
        /// <returns>
        ///     <c>true</c> if a Stump with the specified name already exists; otherwise, <c>false</c>.
        /// </returns>
        public bool StumpNameExists(string stumpName)
        {

            var stumpList = new List<StumpContract>(FindAllContracts());
            var stump = stumpList.Find(s => s.StumpName.Equals(stumpName, StringComparison.OrdinalIgnoreCase));
            var stumpNameExists = stump != null;

            return stumpNameExists;

        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposing && !_disposed)
            {
                _disposed = true;

                if (_lock != null)
                {
                    _lock.Dispose();
                    _lock = null;
                }

            }

        }

        /// <summary>
        ///     Creates a Stump contract from a Stump data entity.
        /// </summary>
        /// <param name="entity">The <see cref="T:Stumps.Server.Data.StumpEntity"/> used to create the contract.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.StumpContract"/> created from the specified <paramref name="entity"/>.
        /// </returns>
        private StumpContract CreateContractFromEntity(StumpEntity entity)
        {

            var contract = new StumpContract
            {
                HttpMethod = entity.HttpMethod,
                MatchBody = LoadFile(entity.MatchBodyFileName),
                MatchBodyContentType = entity.MatchBodyContentType ?? string.Empty,
                MatchBodyIsImage = entity.MatchBodyIsImage,
                MatchBodyIsText = entity.MatchBodyIsText,
                MatchBodyMaximumLength = entity.MatchBodyMaximumLength,
                MatchBodyMinimumLength = entity.MatchBodyMinimumLength,
                MatchBodyText = entity.MatchBodyText,
                MatchHeaders = CreateHttpHeader(entity.MatchHeaders),
                MatchHttpMethod = entity.MatchHttpMethod,
                MatchRawUrl = entity.MatchRawUrl,
                RawUrl = entity.RawUrl,
                Response = new RecordedResponse
                {
                    Body = LoadFile(entity.ResponseBodyFileName),
                    BodyContentType = entity.ResponseBodyContentType,
                    BodyIsImage = entity.ResponseBodyIsImage,
                    BodyIsText = entity.ResponseBodyIsText,
                    Headers = CreateHttpHeader(entity.ResponseHeaders),
                    StatusCode = entity.ResponseStatusCode,
                    StatusDescription = entity.ResponseStatusDescription
                },
                StumpId = entity.StumpId,
                StumpCategory = entity.StumpCategory,
                StumpName = entity.StumpName
            };

            return contract;

        }

        /// <summary>
        ///     Creates a Stump data entity from a Stump contract.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.Proxy.StumpContract"/> used to create the entity.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Data.StumpEntity"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        private StumpEntity CreateEntityFromContract(StumpContract contract)
        {

            var entity = new StumpEntity
            {
                HttpMethod = contract.HttpMethod,
                MatchBodyFileName = string.Empty,
                MatchBodyContentType = contract.MatchBodyContentType ?? string.Empty,
                MatchBodyIsImage = contract.MatchBodyIsImage,
                MatchBodyIsText = contract.MatchBodyIsText,
                MatchBodyMaximumLength = contract.MatchBodyMaximumLength,
                MatchBodyMinimumLength = contract.MatchBodyMinimumLength,
                MatchBodyText = contract.MatchBodyText,
                MatchHeaders = CreateHeaderEntity(contract.MatchHeaders),
                MatchHttpMethod = contract.MatchHttpMethod,
                MatchRawUrl = contract.MatchRawUrl,
                RawUrl = contract.RawUrl,
                ResponseBodyContentType = contract.Response.BodyContentType ?? string.Empty,
                ResponseBodyFileName = string.Empty,
                ResponseBodyIsImage = contract.Response.BodyIsImage,
                ResponseBodyIsText = contract.Response.BodyIsText,
                ResponseHeaders = CreateHeaderEntity(contract.Response.Headers),
                ResponseStatusCode = contract.Response.StatusCode,
                ResponseStatusDescription = contract.Response.StatusDescription,
                StumpId = contract.StumpId,
                StumpCategory = contract.StumpCategory,
                StumpName = contract.StumpName
            };

            return entity;

        }

        /// <summary>
        ///     Creates an array of header entities from an enumerable list of <see cref="T:Stumps.Server.Proxy.HttpHeader"/> objects.
        /// </summary>
        /// <param name="headers">The headers used to create the <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.</param>
        /// <returns>
        ///     An array of <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.
        /// </returns>
        private HeaderEntity[] CreateHeaderEntity(IEnumerable<HttpHeader> headers)
        {

            var headerList = new List<HeaderEntity>();

            foreach (var httpHeader in headers)
            {
                var header = new HeaderEntity
                {
                    Name = httpHeader.Name,
                    Value = httpHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        /// <summary>
        ///     Creates an array of HTTP headers from an enumerable list of <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.
        /// </summary>
        /// <param name="headers">The headers used to create the <see cref="T:Stumps.Server.Proxy.HttpHeader"/> objects.</param>
        /// <returns>
        ///     An array of <see cref="T:Stumps.Server.Proxy.HttpHeader"/> objects.
        /// </returns>
        private HttpHeader[] CreateHttpHeader(IEnumerable<HeaderEntity> headers)
        {

            var headerList = new List<HttpHeader>();

            foreach (var entityHeader in headers)
            {
                var header = new HttpHeader
                {
                    Name = entityHeader.Name,
                    Value = entityHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        /// <summary>
        ///     Creates a Stump from a contract.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.Proxy.StumpContract"/> used to create the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.Proxy.Stump"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        private Stump CreateStumpFromContract(StumpContract contract)
        {

            var stump = new Stump();

            stump.Contract = contract;

            if (contract.MatchRawUrl && !string.IsNullOrWhiteSpace(contract.RawUrl))
            {
                stump.AddRule(new UrlRule(contract.RawUrl));
            }

            if (contract.MatchHttpMethod && !string.IsNullOrWhiteSpace(contract.HttpMethod))
            {
                stump.AddRule(new HttpMethodRule(contract.HttpMethod));
            }

            foreach (var header in contract.MatchHeaders)
            {
                if (!string.IsNullOrWhiteSpace(header.Name) && !string.IsNullOrWhiteSpace(header.Value))
                {
                    stump.AddRule(new HeaderRule(header.Name, header.Value));
                }
            }

            if (contract.MatchBodyMaximumLength != -1)
            {
                stump.AddRule(new BodyLengthRule(contract.MatchBodyMinimumLength, contract.MatchBodyMaximumLength));
            }
            else if (contract.MatchBodyText != null && contract.MatchBodyText.Length > 0)
            {
                stump.AddRule(new BodyContentRule(contract.MatchBodyText));
            }
            else if (contract.MatchBody.Length > 0)
            {
                stump.AddRule(new BodyMatchRule(contract.MatchBody));
            }

            return stump;

        }

        /// <summary>
        ///     Loads all bytes from a specified file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>
        ///     An array of bytes read from the file.
        /// </returns>
        /// <remarks>
        ///     If the file is not found, or cannot be loaded, an empty array is returned.
        /// </remarks>
        private byte[] LoadFile(string fileName)
        {

            var response = new byte[]
            {
            };

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                response = File.ReadAllBytes(fileName);
            }

            return response;

        }

        /// <summary>
        ///     Loads a stump from a specified <see cref="T:Stumps.Server.Proxy.StumpContract"/>.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.Proxy.StumpContract"/> used to create the Stump.</param>
        private void UnwrapAndAddStump(StumpContract contract)
        {

            _lock.EnterWriteLock();

            var stump = CreateStumpFromContract(contract);

            _stumpList.Add(stump);
            _stumpReference.Add(stump.Contract.StumpId, stump);

            _lock.ExitWriteLock();

        }

    }

}