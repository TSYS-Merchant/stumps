namespace Stumps.Data {

    using System.Collections.Generic;

    public interface IDataAccess {

        void ProxyServerCreate(ProxyServerEntity server);

        void ProxyServerDelete(string externalHostName);

        IList<ProxyServerEntity> ProxyServerFindAll();

        StumpEntity StumpCreate(string externalHostName, StumpEntity entity, byte[] matchBody, byte[] responseBody);

        void StumpDelete(string externalHostName, string stumpId);

        IList<StumpEntity> StumpFindAll(string externalHostName);

    }

}
