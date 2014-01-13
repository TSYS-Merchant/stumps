namespace Stumps.Web.ApiModules {

    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    public class ProxyServerStatusModule : NancyModule {

        public ProxyServerStatusModule(IProxyHost proxyHost) {

            Get["/api/proxy/{proxyId}/status"] = _ => {

                var proxyId = (string) _.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new RunningStatusModel {
                    IsRunning = environment.IsRunning
                };

                return Response.AsJson(model);

            };

            Put["/api/proxy/{proxyId}/status"] = _ => {

                var proxyId = (string) _.proxyId;
                var environment = proxyHost.FindProxy(proxyId);

                var model = this.Bind<RunningStatusModel>();

                if ( model.IsRunning && !environment.IsRunning ) {
                    proxyHost.Start(proxyId);
                }
                else if ( !model.IsRunning && environment.IsRunning ) {
                    proxyHost.Shutdown(proxyId);
                }

                return Response.AsJson(model);

            };

        }

    }

}
