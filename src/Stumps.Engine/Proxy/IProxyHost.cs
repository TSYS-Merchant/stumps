namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;

    public interface IProxyHost : IDisposable {

        ProxyEnvironment CreateProxy(string externalHostName, int port, bool useSsl, bool autoStart);

        void DeleteProxy(string proxyId);

        IList<ProxyEnvironment> FindAll();

        ProxyEnvironment FindProxy(string proxyId);

        void Load();

        void Start();

        void Start(string proxyId);

        void Shutdown();

        void Shutdown(string proxyId);

    }

}
