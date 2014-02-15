namespace Stumps.Web.ViewModules
{

    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    public class StumpsOverviewModule : NancyModule
    {

        public StumpsOverviewModule(IProxyHost proxyHost)
        {

            Get["/proxy/{proxyId}/stumps"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var stumpModelArray = new ArrayList();

                var stumpContractList = environment.Stumps.FindAllContracts();
                foreach (var contract in stumpContractList)
                {
                    var stumpModel = new
                    {
                        StumpId = contract.StumpId,
                        StumpName = contract.StumpName
                    };

                    stumpModelArray.Add(stumpModel);
                }

                var model = new
                {
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName,
                    LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                    Stumps = stumpModelArray
                };

                return View["stumpsoverview", model];
            };

        }

    }

}