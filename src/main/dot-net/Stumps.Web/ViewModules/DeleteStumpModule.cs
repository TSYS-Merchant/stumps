namespace Stumps.Web.ViewModules
{

    using System;
    using Nancy;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides support for deleting a Stump from a proxy sever through the Stumps website.
    /// </summary>
    public class DeleteStumpModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.DeleteStumpModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public DeleteStumpModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/proxy/{proxyId}/stumps/{stumpId}/delete"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var stumpId = (string)_.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var model = new
                {
                    StumpName = stump.Contract.StumpName,
                    StumpId = stump.Contract.StumpId,
                    ProxyId = environment.ProxyId
                };

                return View["deletestump", model];

            };

        }

    }

}