namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Stumps.Data;
    using Stumps.Http;
    using Stumps.Utility;

    public class ProxyStumps : IDisposable {

        private readonly List<Stump> _stumpList;
        private readonly Dictionary<string, Stump> _stumpReference;
        private readonly IDataAccess _dataAccess;
        private readonly string _proxyId;
        private ReaderWriterLockSlim _lock;
        private bool _disposed;

        public ProxyStumps(string proxyId, IDataAccess dataAccess) {

            _stumpList = new List<Stump>();
            _stumpReference = new Dictionary<string, Stump>(StringComparer.OrdinalIgnoreCase);

            _dataAccess = dataAccess;
            _proxyId = proxyId;

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        }

        public int Count {
            get { return _stumpList.Count; }
        }

        public StumpContract CreateStump(StumpContract contract) {

            var stumpList = new List<StumpContract>(FindAllContracts());
            var stump = stumpList.Find(s => s.StumpName.Equals(contract.StumpName, StringComparison.OrdinalIgnoreCase));
            
            if (stump != null)
            {
                throw new ArgumentException("Stump name already exists.  Stump names but me unique.");
            }

            if ( contract == null ) {
                throw new ArgumentNullException("contract");
            }

            if ( string.IsNullOrEmpty(contract.StumpId) ) {
                contract.StumpId = RandomGenerator.GenerateIdentifier();
            }

            var entity = CreateEntityFromContract(contract);

            _dataAccess.StumpCreate(_proxyId, entity, contract.MatchBody, contract.Response.Body);

            UnwrapAndAddStump(contract);

            return contract;

        }

        public void DeleteStump(string stumpId) {

            _lock.EnterWriteLock();

            var stump = _stumpReference[stumpId];
            _stumpReference.Remove(stumpId);
            _stumpList.Remove(stump);

            _dataAccess.StumpDelete(_proxyId, stumpId);

            _lock.ExitWriteLock();

        }

        public void Load() {

            var entities = _dataAccess.StumpFindAll(_proxyId);

            foreach ( var entity in entities ) {
                var contract = CreateContractFromEntity(entity);
                UnwrapAndAddStump(contract);
            }

        }

        public IList<StumpContract> FindAllContracts() {

            var stumpContractList = new List<StumpContract>();

            _lock.EnterReadLock();

            foreach ( var stump in _stumpList ) {
                stumpContractList.Add(stump.Contract);
            }

            _lock.ExitReadLock();

            return stumpContractList;

        }

        public Stump FindStump(string stumpId) {

            _lock.EnterReadLock();

            var stump = _stumpReference[stumpId];

            _lock.ExitReadLock();

            return stump;

        }

        public Stump FindStump(IStumpsHttpContext context) {

            Stump foundStump = null;

            _lock.EnterReadLock();

            foreach ( var stump in _stumpList ) {
                if ( stump.IsMatch(context) ) {
                    foundStump = stump;
                    break;
                }
            }

            _lock.ExitReadLock();

            return foundStump;

        }

        private StumpContract CreateContractFromEntity(StumpEntity entity) {

            var contract = new StumpContract {
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
                Response = new RecordedResponse {
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

        private StumpEntity CreateEntityFromContract(StumpContract contract) {

            var entity = new StumpEntity {
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

        private HeaderEntity[] CreateHeaderEntity(IEnumerable<HttpHeader> headers) {

            var headerList = new List<HeaderEntity>();

            foreach ( var httpHeader in headers ) {
                var header = new HeaderEntity {
                    Name = httpHeader.Name,
                    Value = httpHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        private HttpHeader[] CreateHttpHeader(IEnumerable<HeaderEntity> headers) {

            var headerList = new List<HttpHeader>();

            foreach ( var entityHeader in headers ) {
                var header = new HttpHeader {
                    Name = entityHeader.Name,
                    Value = entityHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        private Stump CreateStumpFromContract(StumpContract contract) {

            var stump = new Stump();

            stump.Contract = contract;

            if ( contract.MatchRawUrl && !string.IsNullOrWhiteSpace(contract.RawUrl) ) {
                stump.AddRule(new UrlRule(contract.RawUrl));
            }

            if ( contract.MatchHttpMethod && !string.IsNullOrWhiteSpace(contract.HttpMethod) ) {
                stump.AddRule(new HttpMethodRule(contract.HttpMethod));
            }

            foreach ( var header in contract.MatchHeaders ) {
                if ( !string.IsNullOrWhiteSpace(header.Name) && !string.IsNullOrWhiteSpace(header.Value) ) {
                    stump.AddRule(new HeaderRule(header.Name, header.Value));
                }
            }

            if ( contract.MatchBodyMaximumLength != -1 ) {
                stump.AddRule(new BodyLengthRule(contract.MatchBodyMinimumLength, contract.MatchBodyMaximumLength));
            }
            else if ( contract.MatchBodyText != null && contract.MatchBodyText.Length > 0 ) {
                stump.AddRule(new BodyContentRule(contract.MatchBodyText));
            }
            else if ( contract.MatchBody.Length > 0 ) {
                stump.AddRule(new BodyMatchRule(contract.MatchBody));
            }

            return stump;

        }

        private byte[] LoadFile(string fileName) {

            var response = new byte[] { };

            if ( !string.IsNullOrWhiteSpace(fileName) ) {
                response = File.ReadAllBytes(fileName);
            }

            return response;

        }

        private void UnwrapAndAddStump(StumpContract contract) {

            _lock.EnterWriteLock();

            var stump = CreateStumpFromContract(contract);

            _stumpList.Add(stump);
            _stumpReference.Add(stump.Contract.StumpId, stump);

            _lock.ExitWriteLock();

        }


        #region IDisposable Members

        protected virtual void Dispose(bool disposing) {

            if ( disposing && !_disposed ) {
                _disposed = true;

                if ( _lock != null ) {
                    _lock.Dispose();
                    _lock = null;
                }

            }

        }

        public void Dispose() {

            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        #endregion
    }

}
