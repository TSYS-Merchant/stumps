namespace Stumps.Web.ViewModules {

    using System.Globalization;
    using Nancy;
    using Stumps.Web.Models;
    using Stumps.Proxy;

    public class StumpEditorModule : NancyModule {

        public StumpEditorModule(IProxyHost proxyHost) {

            Get["/proxy/{proxyId}/recording/{recordIndex}/newstump"] = _ => {
                var proxyId = (string) _.proxyId;
                var recordIndex = (int) _.recordIndex;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new {
                    StumpName = "Stump - " + System.Environment.TickCount.ToString(CultureInfo.InvariantCulture),
                    Origin = (int) StumpOrigin.RecordedContext,
                    StumpId = string.Empty,
                    ProxyId = environment.ProxyId,
                    ExternalHostName = (environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName),
                    LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                    BackUrl = "/proxy/" + proxyId + "/recordings",
                    CreateButtonText = "Create New Stump",
                    LoadRecord = true,
                    LoadStump = false,
                    RecordIndex = recordIndex
                };

                return View["stumpeditor", model];
            };

            Get["/proxy/{proxyId}/stumps/{stumpId}"] = _ => {
                var proxyId = (string) _.proxyId;
                var stumpId = (string) _.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var model = new {
                    StumpName = stump.Contract.StumpName,
                    Origin = (int) StumpOrigin.ExistingStump,
                    StumpId = stump.Contract.StumpId,
                    ProxyId = environment.ProxyId,
                    ExternalHostName = (environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName),
                    LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                    BackUrl = "/proxy/" + proxyId + "/stumps",
                    CreateButtonText = "Save Stump",
                    LoadRecord = false,
                    LoadStump = true,
                    RecordIndex = 0
                };

                return View["stumpeditor", model];
            };

        }

    }

}
