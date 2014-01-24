namespace Stumps.Web.ApiModules {

    using System;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;
    using Stumps.Utility;

    public class PortAvailableModule : NancyModule
    {
        public PortAvailableModule(IProxyHost proxyHost)
            : base("/api") {

            if (proxyHost == null) {
                    throw new ArgumentNullException("proxyHost");
            }

            Get["/portAvailable/{port}"] = _ =>
            {
                var port = _.port;
                
                var model = new PortAvailableModel
                {
                    PortAvailable = !NetworkUtility.IsPortBeingUsed(port)
                };

                return Response.AsJson(model);
            };
        }
    }
}
