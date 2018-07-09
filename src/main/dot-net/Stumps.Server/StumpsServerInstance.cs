namespace Stumps.Server
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Stumps.Server.Data;
    using Stumps.Server.Utility;

    /// <summary>
    /// A class that represents an the environment and configuration of a Stumps server.
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

        private bool _recordTraffic;
        private bool _lastKnownStumpsEnabledState;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsServerInstance"/> class.
        /// </summary>
        /// <param name="serverFactory">The factory used to initialize new server instances.</param>
        /// <param name="serverId">The unique identifier of the Stumps server.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        public StumpsServerInstance(IServerFactory serverFactory, string serverId, IDataAccess dataAccess)
        {
            _serverFactory = serverFactory ?? throw new ArgumentNullException(nameof(serverFactory));

            this.ServerId = serverId;

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            _dataAccess = dataAccess;

            // Setup the objects needed to keep track of Stumps.
            _stumpList = new List<StumpContract>();
            _stumpReference = new Dictionary<string, StumpContract>(StringComparer.OrdinalIgnoreCase);

            // Initialize the server
            InitializeServer();

            // Initialize the Stumps
            InitializeStumps();
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="StumpsServerInstance"/> class.
        /// </summary>
        ~StumpsServerInstance() => Dispose(false);

        /// <summary>
        ///     Gets or sets a value indicating whether to automatically start the instance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the instance should automatically; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStart
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the host name of the remote server.
        /// </summary>
        /// <value>
        ///     The host name of the remote server.
        /// </value>
        public string RemoteServerHostName
        {
            get;
            set;
        }

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
                var isServerRunning = _server != null && _server.IsRunning;
                return isServerRunning;
            }
        }

        /// <summary>
        ///     Gets or sets the port the Stumps server is listening on for incoming HTTP requests.
        /// </summary>
        /// <value>
        ///     The port the Stumps server is listening on for incoming HTTP requests.
        /// </value>
        public int ListeningPort
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the recorded HTTP requests and responses.
        /// </summary>
        /// <value>
        ///     The recorded HTTP requests and responses.
        /// </value>
        public Recordings Recordings
        {
            get;
        } = new Recordings();

        /// <summary>
        /// Gets or sets a value indicating whether to record all traffic.
        /// </summary>
        /// <value>
        ///   <c>true</c> if traffic should be recorded; otherwise, <c>false</c>.
        /// </value>
        public bool RecordTraffic
        {
            get => _recordTraffic;
            set 
            { 
                if (value)
                {
                    _lastKnownStumpsEnabledState = this.StumpsEnabled;

                    if (this.RecordingBehavior == RecordingBehavior.DisableStumps)
                    {
                        this.StumpsEnabled = false;
                    }
                }
                else
                {
                    this.StumpsEnabled = _lastKnownStumpsEnabledState;
                }

                _recordTraffic = value;
            }
        }

        /// <summary>
        ///     Gets or sets the behavior of the instance when recording traffic.
        /// </summary>
        /// <value>
        ///     The behavior of the instance when recording traffic.
        /// </value>
        public RecordingBehavior RecordingBehavior
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use stumps when serving requests.
        /// </summary>
        /// <value>
        ///   <c>true</c> to use stumps when serving requests; otherwise, <c>false</c>.
        /// </value>
        public bool StumpsEnabled
        {
            get => _server.StumpsEnabled;
            set => _server.StumpsEnabled = value;
        }

        /// <summary>
        ///     Gets the number of requests served by the remote server.
        /// </summary>
        /// <value>
        ///     The number of requests served by the remote server.
        /// </value>
        public int RequestsServedByRemoteServer
        {
            get => _server.RequestsServedByRemoteHost;
        }

        /// <summary>
        ///     Gets the number requests served with a Stump.
        /// </summary>
        /// <value>
        ///     The number of requests served with a Stumps.
        /// </value>
        public int RequestsServedWithStump
        {
            get => _server.RequestsServedWithStump;
        }

        /// <summary>
        ///     Gets or sets the unique identifier for the server.
        /// </summary>
        /// <value>
        ///     The unique identifier for the server.
        /// </value>
        public string ServerId
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        ///     The count of Stumps in the collection.
        /// </value>
        public int StumpCount
        {
            get => _stumpList.Count;
        }

        /// <summary>
        ///     Gets the total number of requests served.
        /// </summary>
        /// <value>
        ///     The total number of requests served.
        /// </value>
        public int TotalRequestsServed
        {
            get => _server.TotalRequestsServed;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the remote server requires SSL.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the remote server requires SSL; otherwise, <c>false</c>.
        /// </value>
        public bool UseSsl
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether use HTTPS for incoming connections rather than HTTP.
        /// </summary>
        /// <value>
        ///     <c>true</c> to use HTTPS for incoming HTTP connections rather than HTTP.
        /// </value>
        public bool UseHttpsForIncomingConnections
        {
            get;
            set;
        }

        /// <summary>
        ///     Creates a new Stump.
        /// </summary>
        /// <param name="contract">The contract used to create the Stump.</param>
        /// <returns>
        ///     An updated <see cref="StumpContract"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="contract"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">A stump with the same name already exists.</exception>
        public StumpContract CreateStump(StumpContract contract)
        {
            contract = contract ?? throw new ArgumentNullException(nameof(contract));

            if (string.IsNullOrEmpty(contract.StumpId))
            {
                contract.StumpId = RandomGenerator.GenerateIdentifier();
            }

            if (this.StumpNameExists(contract.StumpName))
            {
                throw new ArgumentException(Resources.StumpNameUsedError);
            }

            var entity = ContractEntityBinding.CreateEntityFromContract(contract);

            _dataAccess.StumpCreate(this.ServerId, entity, contract.OriginalRequest.GetBody(), contract.OriginalResponse.GetBody(), contract.Response.GetBody());

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
        ///     A generic list of all <see cref="StumpContract"/> objects.
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
        ///     A <see cref="StumpContract"/> with the specified <paramref name="stumpId"/>.
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
        ///     Stops this instance of the Stumps server.
        /// </summary>
        public void Shutdown()
        {
            if (_server != null)
            {
                _server.Shutdown();
            }
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
                    this.Shutdown();
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
        ///     Initializes the Stumps server controlled by this instance.
        /// </summary>
        private void InitializeServer()
        {
            // Find the persisted server entity 
            var entity = _dataAccess.ServerFind(this.ServerId);
            this.AutoStart = entity.AutoStart;
            this.RemoteServerHostName = entity.RemoteServerHostName;
            this.ListeningPort = entity.Port;
            this.UseSsl = entity.UseSsl;
            this.UseHttpsForIncomingConnections = entity.UseHttpsForIncomingConnections;

            this.RecordingBehavior = entity.DisableStumpsWhenRecording
                                         ? RecordingBehavior.DisableStumps
                                         : RecordingBehavior.LeaveStumpsUnchanged;

            if (!string.IsNullOrWhiteSpace(this.RemoteServerHostName))
            {
                var pattern = this.UseSsl
                                  ? StumpsServerInstance.SecureUriFormat
                                  : StumpsServerInstance.InsecureUriFormat;

                var uriString = string.Format(CultureInfo.InvariantCulture, pattern, this.RemoteServerHostName);

                var uri = new Uri(uriString);

                _server = _serverFactory.CreateServer(this.ListeningPort, uri);
                _server.UseHttpsForIncomingConnections = this.UseHttpsForIncomingConnections;
            }
            else
            {
                // TODO: Choose which method to use for the fallback when no remote server is available.
                _server = _serverFactory.CreateServer(this.ListeningPort, FallbackResponse.Http503ServiceUnavailable);
            }

            _server.RequestFinished += (o, e) =>
            {
                if (this.RecordTraffic)
                {
                    this.Recordings.Add(e.Context);
                }
            };
        }

        /// <summary>
        ///     Initializes the Stumps for the server.
        /// </summary>
        private void InitializeStumps()
        {
            var entities = _dataAccess.StumpFindAll(this.ServerId);

            foreach (var entity in entities)
            {
                var contract = ContractEntityBinding.CreateContractFromEntity(this.ServerId, entity, _dataAccess);
                UnwrapAndAddStump(contract);
            }
        }
        
        /// <summary>
        ///     Loads a stump from a specified <see cref="StumpContract"/>.
        /// </summary>
        /// <param name="contract">The <see cref="StumpContract"/> used to create the Stump.</param>
        private void UnwrapAndAddStump(StumpContract contract)
        {
            _lock.EnterWriteLock();

            var stump = ContractBindings.CreateStumpFromContract(contract);

            _stumpList.Add(contract);
            _stumpReference.Add(stump.StumpId, contract);
            _server.AddStump(stump);

            _lock.ExitWriteLock();
        }
    }
}