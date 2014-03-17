namespace Stumps.Web.ApiModules
{

    using System;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Utility;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the discovering available ports on the server through the REST API.
    /// </summary>
    public class PortAvailableModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.PortAvailableModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
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
                    PortAvailable = !NetworkInformation.IsPortBeingUsed(port),
                };

                return Response.AsJson(model);
            };
        }

    }

}