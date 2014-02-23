namespace Stumps.Web.ViewModules
{

    using System;
    using Nancy;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides support for deleting a proxy server through the Stumps website.
    /// </summary>
    public class DeleteWebsiteModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.DeleteWebsiteModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public DeleteWebsiteModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/proxy/{proxyId}/delete"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new
                {
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.ExternalHostName
                };

                return View["deletewebsite", model];
            };

        }

    }

}