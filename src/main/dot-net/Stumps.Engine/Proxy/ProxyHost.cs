namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using System.Collections.Concurrent;
    using System.Globalization;
    using System.Net;
    using Stumps.Logging;
    using Stumps.Data;
    using Stumps.Utility;
    using System.Text.RegularExpressions;

    public class ProxyHost : IProxyHost {

        private readonly ConcurrentDictionary<string, ProxyServer> _proxies;
        private readonly ILogger _logger;
        private readonly IDataAccess _dataAccess;
        private bool _disposed;
        private static CultureInfo cultureInfo;

        public ProxyHost(ILogger logger, IDataAccess dataAccess) {

            if ( logger == null ) {
                throw new ArgumentNullException("logger");
            }

            if ( dataAccess == null ) {
                throw new ArgumentNullException("dataAccess");
            }

            _logger = logger;
            _dataAccess = dataAccess;

            _proxies = new ConcurrentDictionary<string, ProxyServer>(StringComparer.OrdinalIgnoreCase);

        }

        public static bool containsProtocol(string hostName)
        {
            try
            {
                Uri uri = new Uri(hostName);
                if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                    return true;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static bool isHttps(string hostName)
        {
            try
            {
                Uri uri = new Uri(hostName);
                if (uri.Scheme == Uri.UriSchemeHttps)
                    return true;
                else 
                    return false;
            }
            catch
            {
                return false;
            }
        }
                
        public ProxyEnvironment CreateProxy(string externalHostName, int port, bool useSsl, bool autoStart) {
            
            if ( string.IsNullOrWhiteSpace(externalHostName) ) {
                throw new ArgumentNullException("externalHostName");
            }

            if ( port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort ) {
                throw new ArgumentOutOfRangeException("port");
            }
            
            // If the user mistakenly puts in http:// or https://, grab just the domain.  If it's https://, then the UseSsl value will be automatically set to true.
            if(containsProtocol(externalHostName)) 
            {
                Uri externalHost = new Uri(externalHostName);
                string domain = externalHost.Host;

                if (isHttps(externalHostName))
                {
                    externalHostName = domain;
                    useSsl = true;
                }
                else 
                {
                    externalHostName = domain;
                }
             }

            var proxyEntity = new ProxyServerEntity() {
                AutoStart = autoStart,
                ExternalHostName = externalHostName,
                Port = port,
                UseSsl = useSsl,
                ProxyId = RandomGenerator.GenerateIdentifier()
            };

            _dataAccess.ProxyServerCreate(proxyEntity);

            unwrapAndRegisterProxy(proxyEntity);

            var server = _proxies[proxyEntity.ProxyId];

            if ( autoStart ) {
                server.Start();
            }

            return server.Environment;
        }

        public void DeleteProxy(string proxyId) {

            if ( string.IsNullOrWhiteSpace(proxyId) ) {
                throw new ArgumentNullException("proxyId");
            }

            if ( _proxies.ContainsKey(proxyId) ) {
                var hostName = _proxies[proxyId].Environment.ExternalHostName;

                _proxies[proxyId].Stop();
                _proxies[proxyId].Dispose();

                ProxyServer server = null;
                _proxies.TryRemove(proxyId, out server);

                _dataAccess.ProxyServerDelete(hostName);
            }

        }

        public IList<ProxyEnvironment> FindAll() {

            var environmentList = new List<ProxyEnvironment>();
            var pairs = _proxies.ToArray();

            foreach ( var pair in pairs ) {
                environmentList.Add(pair.Value.Environment);
            }

            return environmentList;

        }

        public ProxyEnvironment FindProxy(string proxyId) {

            ProxyServer server = null;
            ProxyEnvironment environment = null;

            _proxies.TryGetValue(proxyId, out server);

            if ( server != null ) {
                environment = server.Environment;
            }

            return environment;

        }

        public void Load() {

            var proxyEntities = _dataAccess.ProxyServerFindAll();

            foreach ( var proxyEntity in proxyEntities ) {
                unwrapAndRegisterProxy(proxyEntity);
            }

        }

        public void Start() {

            foreach ( var server in _proxies ) {
                if ( server.Value.Environment.AutoStart ) {
                    server.Value.Start();
                }
            }

        }

        public void Start(string proxyId) {

            if ( string.IsNullOrWhiteSpace(proxyId) ) {
                throw new ArgumentNullException("proxyId");
            }

            ProxyServer server = null;
            _proxies.TryGetValue(proxyId, out server);

            if ( server != null ) {
                server.Start();
            }

        }

        public void Shutdown() {

            foreach ( var keyPair in _proxies ) {
                keyPair.Value.Stop();
            }

        }

        public void Shutdown(string proxyId) {

            if ( string.IsNullOrWhiteSpace(proxyId) ) {
                throw new ArgumentNullException("proxyId");
            }

            ProxyServer server = null;
            _proxies.TryGetValue(proxyId, out server);

            if ( server != null ) {
                server.Stop();
            }

        }

        private void unwrapAndRegisterProxy(ProxyServerEntity entity) {

            var environment = new ProxyEnvironment(entity.ExternalHostName, _dataAccess) {
                Port = entity.Port,
                UseSsl = entity.UseSsl,
                AutoStart = entity.AutoStart,
                ProxyId = entity.ProxyId
            };

            var server = new ProxyServer(environment, _logger);

            environment.Stumps.Load();

            _proxies.AddOrUpdate(environment.ProxyId, server, (key, oldServer) => server);

        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing) {

            if ( disposing && !_disposed ) {

                _disposed = true;

                foreach ( var keyPair in _proxies ) {
                    if ( keyPair.Value != null ) {
                        keyPair.Value.Dispose();
                    }
                }

                _proxies.Clear();

            }

        }

        public void Dispose() {

            if ( !_disposed ) {

                this.Dispose(true);
                GC.SuppressFinalize(this);

            }

        }

        #endregion

    }

}
