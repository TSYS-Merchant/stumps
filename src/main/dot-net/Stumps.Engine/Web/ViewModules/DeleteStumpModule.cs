namespace Stumps.Web.ViewModules
{

    using Nancy;
    using Stumps.Proxy;

    public class DeleteStumpModule : NancyModule
    {

        public DeleteStumpModule(IProxyHost proxyHost)
        {

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