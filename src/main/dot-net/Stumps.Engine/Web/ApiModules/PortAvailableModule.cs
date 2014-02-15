namespace Stumps.Web.ApiModules
{

    using System;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Utility;
    using Stumps.Web.Models;

    public class PortAvailableModule : NancyModule
    {

        public PortAvailableModule(IProxyHost proxyHost) : base("/api")
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/portAvailable/{port}"] = _ =>
            {
                var port = _.port;

                var model = new PortAvailableModel
                {
                    PortAvailable = !NetworkUtility.IsPortBeingUsed(port),
                };

                return Response.AsJson(model);
            };
        }

    }

}