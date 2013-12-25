namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using Stumps.Data;
    using Stumps.Http;
    using Stumps.Utility;

    public class ProxyStumps {

        private readonly List<Stump> _stumpList;
        private readonly Dictionary<string, Stump> _stumpReference;
        private readonly ReaderWriterLockSlim _lock;
        private readonly IDataAccess _dataAccess;
        private readonly string _externalHostName;

        public ProxyStumps(string externalHostName, IDataAccess dataAccess) {

            _stumpList = new List<Stump>();
            _stumpReference = new Dictionary<string, Stump>(StringComparer.OrdinalIgnoreCase);

            _dataAccess = dataAccess;
            _externalHostName = externalHostName;

            _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        }

        public int Count {
            get { return _stumpList.Count; }
        }

        public StumpContract CreateStump(StumpContract contract) {

            if ( contract == null ) {
                throw new ArgumentNullException("contract");
            }

            if ( string.IsNullOrEmpty(contract.StumpId) ) {
                contract.StumpId = RandomGenerator.GenerateIdentifier();
            }

            var entity = createEntityFromContract(contract);

            _dataAccess.StumpCreate(_externalHostName, entity, contract.MatchBody, contract.Response.Body);

            unwrapAndAddStump(contract);

            return contract;

        }

        public void DeleteStump(string stumpId) {

            _lock.EnterWriteLock();

            var stump = _stumpReference[stumpId];
            _stumpReference.Remove(stumpId);
            _stumpList.Remove(stump);

            _dataAccess.StumpDelete(_externalHostName, stumpId);

            _lock.ExitWriteLock();

        }

        public void Load() {

            var entities = _dataAccess.StumpFindAll(_externalHostName);

            foreach ( var entity in entities ) {
                var contract = createContractFromEntity(entity);
                unwrapAndAddStump(contract);
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

        private StumpContract createContractFromEntity(StumpEntity entity) {

            var contract = new StumpContract() {
                HttpMethod = entity.HttpMethod,
                MatchBody = loadFile(entity.MatchBodyFileName),
                MatchBodyContentType = entity.MatchBodyContentType ?? string.Empty,
                MatchBodyIsImage = entity.MatchBodyIsImage,
                MatchBodyIsText = entity.MatchBodyIsText,
                MatchBodyMaximumLength = entity.MatchBodyMaximumLength,
                MatchBodyMinimumLength = entity.MatchBodyMinimumLength,
                MatchBodyText = entity.MatchBodyText,
                MatchHeaders = createHttpHeader(entity.MatchHeaders),
                MatchHttpMethod = entity.MatchHttpMethod,
                MatchRawUrl = entity.MatchRawUrl,
                RawUrl = entity.RawUrl,
                Response = new RecordedResponse() {
                    Body = loadFile(entity.ResponseBodyFileName),
                    BodyContentType = entity.ResponseBodyContentType,
                    BodyIsImage = entity.ResponseBodyIsImage,
                    BodyIsText = entity.ResponseBodyIsText,
                    Headers = createHttpHeader(entity.ResponseHeaders),
                    StatusCode = entity.ResponseStatusCode,
                    StatusDescription = entity.ResponseStatusDescription
                },
                StumpId = entity.StumpId,
                StumpCategory = entity.StumpCategory,
                StumpName = entity.StumpName
            };

            return contract;

        }

        private StumpEntity createEntityFromContract(StumpContract contract) {

            var entity = new StumpEntity() {
                HttpMethod = contract.HttpMethod,
                MatchBodyFileName = string.Empty,
                MatchBodyContentType = contract.MatchBodyContentType ?? string.Empty,
                MatchBodyIsImage = contract.MatchBodyIsImage,
                MatchBodyIsText = contract.MatchBodyIsText,
                MatchBodyMaximumLength = contract.MatchBodyMaximumLength,
                MatchBodyMinimumLength = contract.MatchBodyMinimumLength,
                MatchBodyText = contract.MatchBodyText,
                MatchHeaders = createHeaderEntity(contract.MatchHeaders),
                MatchHttpMethod = contract.MatchHttpMethod,
                MatchRawUrl = contract.MatchRawUrl,
                RawUrl = contract.RawUrl,
                ResponseBodyContentType = contract.Response.BodyContentType ?? string.Empty,
                ResponseBodyFileName = string.Empty,
                ResponseBodyIsImage = contract.Response.BodyIsImage,
                ResponseBodyIsText = contract.Response.BodyIsText,
                ResponseHeaders = createHeaderEntity(contract.Response.Headers),
                ResponseStatusCode = contract.Response.StatusCode,
                ResponseStatusDescription = contract.Response.StatusDescription,
                StumpId = contract.StumpId,
                StumpCategory = contract.StumpCategory,
                StumpName = contract.StumpName
            };

            return entity;

        }

        private HeaderEntity[] createHeaderEntity(IEnumerable<HttpHeader> headers) {

            var headerList = new List<HeaderEntity>();

            foreach ( var httpHeader in headers ) {
                var header = new HeaderEntity() {
                    Name = httpHeader.Name,
                    Value = httpHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        private HttpHeader[] createHttpHeader(IEnumerable<HeaderEntity> headers) {

            var headerList = new List<HttpHeader>();

            foreach ( var entityHeader in headers ) {
                var header = new HttpHeader() {
                    Name = entityHeader.Name,
                    Value = entityHeader.Value
                };

                headerList.Add(header);
            }

            return headerList.ToArray();

        }

        private Stump createStumpFromContract(StumpContract contract) {

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

        private byte[] loadFile(string fileName) {

            var response = new byte[] { };

            if ( !string.IsNullOrWhiteSpace(fileName) ) {
                response = File.ReadAllBytes(fileName);
            }

            return response;

        }

        private void unwrapAndAddStump(StumpContract contract) {

            _lock.EnterWriteLock();

            var stump = createStumpFromContract(contract);

            _stumpList.Add(stump);
            _stumpReference.Add(stump.Contract.StumpId, stump);

            _lock.ExitWriteLock();

        }

    }

}
