namespace Stumps.Server
{

    using System;
    using System.Linq;
    using Stumps.Server.Data;
    using Stumps.Server.Proxy;
    using Stumps.Server.Utility;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Stumps.Rules;

    /// <summary>
    /// A class that represents an the environment and configuration of a proxy server.
    /// </summary>
    public class StumpsServerInstance : IDisposable
    {

        /// <summary>
        ///     The format for a URI for an insecure HTTP connection.
        /// </summary>
        private const string InsecureUriFormat = "http://{0}/";

        /// <summary>
        ///     The format for a URI for a secure HTTP connection.
        /// </summary>
        private const string SecureUriFormat = "https://{0}/";

        private readonly IServerFactory _serverFactory;

        private readonly List<StumpContract> _stumpList;
        private readonly Dictionary<string, StumpContract> _stumpReference;

        private readonly IDataAccess _dataAccess;
        private IStumpsServer _server;
        private bool _disposed;
        private ReaderWriterLockSlim _lock;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Server.StumpsServerInstance"/> class.
        /// </summary>
        /// <param name="serverFactory">The factory used to initialize new server instances.</param>
        /// <param name="proxyId">The unique identifier of the proxy.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        public StumpsServerInstance(IServerFactory serverFactory, string proxyId, IDataAccess dataAccess)
        {

            if (serverFactory == null)
            {
                throw new ArgumentNullException("serverFactory");
            }

            _serverFactory = serverFactory;

            this.ServerId = proxyId;

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _dataAccess = dataAccess;

            // Setup the objects needed to keep track of Stumps.
            _stumpList = new List<StumpContract>();
            _stumpReference = new Dictionary<string, StumpContract>(StringComparer.OrdinalIgnoreCase);

            // Setup the recordings maintained by the server instance.
            this.Recordings = new ProxyRecordings();

            // Initialize the server
            InitializeServer();

            // Initialize the Stumps
            InitializeStumps();

        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="T:Stumps.Server.StumpsServerInstance"/> class.
        /// </summary>
        ~StumpsServerInstance()
        {
            this.Dispose(false);
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically start the instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance should automatically; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart { get; set; }

        /// <summary>
        ///     Gets or sets the name of the external host.
        /// </summary>
        /// <value>
        ///     The name of the external host.
        /// </value>
        public string ExternalHostName { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the instance is running.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                var isServerRunning = _server != null && this._server.IsRunning;
                return isServerRunning;
            }
        }

        /// <summary>
        ///     Gets or sets the port the Stumps server is listening on for incomming HTTP requests.
        /// </summary>
        /// <value>
        ///     The port the Stumps server is listening on for incomming HTTP requests.
        /// </value>
        public int ListeningPort { get; set; }

        /// <summary>
        ///     Gets the recorded HTTP requests and responses.
        /// </summary>
        /// <value>
        ///     The recorded HTTP requests and responses.
        /// </value>
        public ProxyRecordings Recordings { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to record all traffic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if traffic should be recorded; otherwise, <c>false</c>.
        /// </value>
        public bool RecordTraffic { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId { get; set; }

        /// <summary>
        ///     Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        ///     The count of Stumps in the collection.
        /// </value>
        public int StumpCount
        {
            get { return _stumpList.Count; }
        }
        
        /// <summary>
        ///     Gets or sets a value indicating whether the exernal host requires SSL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the external host requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl { get; set; }

        /// <summary>
        ///     Creates a new Stump.
        /// </summary>
        /// <param name="contract">The contract used to create the Stump.</param>
        /// <returns>
        ///     An updated <see cref="T:Stumps.Server.StumpContract"/>.
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

            _dataAccess.StumpCreate(this.ServerId, entity, contract.MatchBody, contract.Response.GetBody());

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
            _server.DeleteStump(stumpId);

            _dataAccess.StumpDelete(this.ServerId, stumpId);

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
        ///     Finds a list of all Stump contracts.
        /// </summary>
        /// <returns>
        ///     A generic list of all <see cref="T:Stumps.Server.StumpContract"/> objects.
        /// </returns>
        public IList<StumpContract> FindAllContracts()
        {

            _lock.EnterReadLock();

            var stumpContractList = this._stumpList.ToList();

            _lock.ExitReadLock();

            return stumpContractList;

        }

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Server.StumpContract"/> with the specified <paramref name="stumpId"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stump is not found.
        /// </remarks>
        public StumpContract FindStump(string stumpId)
        {

            _lock.EnterReadLock();

            var stump = _stumpReference[stumpId];
            _lock.ExitReadLock();
            return stump;

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
        ///     Starts this instance of the Stumps server.
        /// </summary>
        public void Start()
        {

            if (_server != null)
            {
                _server.Start();
            }

        }

        /// <summary>
        ///     Stops this instance of the Stumps server.
        /// </summary>
        public void Stop()
        {

            if (_server != null)
            {
                _server.Stop();
            }

        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {

            if (!_disposed)
            {

                _disposed = true;

                if (this.IsRunning)
                {
                    this.Stop();
                }

                if (_server != null)
                {
                    _server.Dispose();
                }

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
        ///     A <see cref="T:Stumps.Server.StumpContract"/> created from the specified <paramref name="entity"/>.
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
                StumpId = entity.StumpId,
                StumpCategory = entity.StumpCategory,
                StumpName = entity.StumpName
            };

            // Setup the response for the contract
            var response = new ContractHttpResponse
            {
                StatusCode = entity.ResponseStatusCode,
                StatusDescription = entity.ResponseStatusDescription,
                BodyIsImage = entity.ResponseBodyIsImage,
                BodyIsText = entity.ResponseBodyIsText
            };

            response.AppendToBody(LoadFile(entity.ResponseBodyFileName));

            foreach (var header in entity.ResponseHeaders)
            {
                response.Headers.AddOrUpdate(header.Name, header.Value);
            }

            contract.Response = response;

            return contract;

        }


        /// <summary>
        ///     Creates a Stump data entity from a Stump contract.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.StumpContract"/> used to create the entity.</param>
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
                ResponseBodyContentType = contract.Response.Headers["content-type"] ?? string.Empty,
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
        ///     Creates an array of header entities from an enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects.
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
        ///     Creates an array of header entities from an enumerable list of <see cref="T:Stumps.Server.HttpHeader"/> objects.
        /// </summary>
        /// <param name="headers">The headers used to create the <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.</param>
        /// <returns>
        ///     An array of <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.
        /// </returns>
        private HeaderEntity[] CreateHeaderEntity(IHeaderDictionary headers)
        {

            var headerList = new List<HeaderEntity>();

            foreach (var headerName in headers.HeaderNames)
            {
                var header = new HeaderEntity
                {
                    Name = headerName,
                    Value = headers[headerName]
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        /// <summary>
        ///     Creates an array of HTTP headers from an enumerable list of <see cref="T:Stumps.Server.Data.HeaderEntity"/> objects.
        /// </summary>
        /// <param name="headers">The headers used to create the <see cref="T:Stumps.Server.HttpHeader"/> objects.</param>
        /// <returns>
        ///     An array of <see cref="T:Stumps.Server.HttpHeader"/> objects.
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
        /// <param name="contract">The <see cref="T:Stumps.Server.StumpContract"/> used to create the Stump.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Stump"/> created from the specified <paramref name="contract"/>.
        /// </returns>
        private Stump CreateStumpFromContract(StumpContract contract)
        {

            var stump = new Stump(contract.StumpId);

            // stump.Contract = contract;

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
        ///     Initializes the Stumps server controlled by this instance.
        /// </summary>
        private void InitializeServer()
        {

            // Find the persisted server entity 
            var entity = _dataAccess.ProxyServerFind(this.ServerId);
            this.AutoStart = entity.AutoStart;
            this.ExternalHostName = entity.ExternalHostName;
            this.ListeningPort = entity.Port;
            this.UseSsl = entity.UseSsl;

            if (!string.IsNullOrWhiteSpace(this.ExternalHostName))
            {
                var pattern = this.UseSsl
                                  ? StumpsServerInstance.SecureUriFormat
                                  : StumpsServerInstance.InsecureUriFormat;

                var uriString = string.Format(pattern, this.ExternalHostName);

                var uri = new Uri(uriString);

                _server = _serverFactory.CreateServer(this.ListeningPort, uri);
            }
            else
            {
                // TODO: Choose which method to use for the fallback when no proxy is available.
                _server = _serverFactory.CreateServer(this.ListeningPort, ServerDefaultResponse.Http503ServiceUnavailable);
            }

        }

        /// <summary>
        ///     Initializes the Stumps for the server.
        /// </summary>
        private void InitializeStumps()
        {
            var entities = _dataAccess.StumpFindAll(this.ServerId);

            foreach (var entity in entities)
            {
                var contract = CreateContractFromEntity(entity);
                UnwrapAndAddStump(contract);
            }

        }

        /// <summary>
        ///     Loads a stump from a specified <see cref="T:Stumps.Server.StumpContract"/>.
        /// </summary>
        /// <param name="contract">The <see cref="T:Stumps.Server.StumpContract"/> used to create the Stump.</param>
        private void UnwrapAndAddStump(StumpContract contract)
        {

            _lock.EnterWriteLock();

            var stump = CreateStumpFromContract(contract);

            _stumpList.Add(contract);
            _stumpReference.Add(stump.StumpId, contract);
            _server.AddStump(stump);

            _lock.ExitWriteLock();

        }

    }

}