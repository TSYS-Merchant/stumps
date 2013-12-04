namespace Stumps.Web.ViewModules {

    using Nancy;
    using Stumps.Proxy;

    public class DeleteWebsiteModule : NancyModule {

        public DeleteWebsiteModule(IProxyHost proxyHost) {

            Get["/proxy/{proxyId}/delete"] = _ => {
                var proxyId = (string) _.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new {
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.ExternalHostName
                };

                return View["deletewebsite", model];
            };

        }

    }

}
