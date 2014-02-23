namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the managing proxy servers through the REST API.
    /// </summary>
    public sealed class ProxyServerModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.ProxyServerModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public ProxyServerModule(IProxyHost proxyHost) : base("/api")
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/proxy"] = _ =>
            {

                var modelList = new List<ProxyServerDetailsModel>();
                var environmentList = proxyHost.FindAll();

                foreach (var environment in environmentList)
                {
                    var model = new ProxyServerDetailsModel
                    {
                        AutoStart = environment.AutoStart,
                        ExternalHostName = environment.ExternalHostName,
                        IsRunning = environment.IsRunning,
                        Port = environment.Port,
                        RecordCount = environment.Recordings.Count,
                        RecordTraffic = environment.RecordTraffic,
                        RequestsServed = environment.RequestsServed,
                        StumpsCount = environment.Stumps.Count,
                        StumpsServed = environment.StumpsServed,
                        ProxyId = environment.ProxyId,
                        UseSsl = environment.UseSsl
                    };

                    modelList.Add(model);
                }

                return Response.AsJson(modelList);

            };

            Post["/proxy"] = _ =>
            {
                var model = this.Bind<ProxyServerModel>();

                proxyHost.CreateProxy(model.ExternalHostName, model.Port, model.UseSsl, model.AutoStart);

                return HttpStatusCode.Created;
            };

            Delete["/proxy/{proxyId}"] = _ =>
            {
                var proxyId = (string)_.proxyId;

                proxyHost.DeleteProxy(proxyId);

                return HttpStatusCode.OK;
            };

        }

    }

}