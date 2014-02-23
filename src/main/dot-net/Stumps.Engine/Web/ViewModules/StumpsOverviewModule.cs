namespace Stumps.Web.ViewModules
{

    using System;
    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides support the Stumps overview webpage of the Stumps website.
    /// </summary>
    public class StumpsOverviewModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.StumpsOverviewModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public StumpsOverviewModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

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