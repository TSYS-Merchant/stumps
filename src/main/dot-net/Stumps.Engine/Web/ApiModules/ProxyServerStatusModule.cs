namespace Stumps.Web.ApiModules
{

    using System;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the discovering status of a proxy server through the RESET API.
    /// </summary>
    public class ProxyServerStatusModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.ProxyServerStatusModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public ProxyServerStatusModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/api/proxy/{proxyId}/status"] = _ =>
            {

                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new RunningStatusModel
                {
                    IsRunning = environment.IsRunning
                };

                return Response.AsJson(model);

            };

            Put["/api/proxy/{proxyId}/status"] = _ =>
            {

                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = this.Bind<RunningStatusModel>();

                if (model.IsRunning && !environment.IsRunning)
                {
                    proxyHost.Start(proxyId);
                }
                else if (!model.IsRunning && environment.IsRunning)
                {
                    proxyHost.Shutdown(proxyId);
                }

                return Response.AsJson(model);

            };

        }

    }

}