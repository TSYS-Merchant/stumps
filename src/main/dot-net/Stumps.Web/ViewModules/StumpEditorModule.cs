namespace Stumps.Web.ViewModules
{

    using System;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for creating or editing Stumps.
    /// </summary>
    public class StumpEditorModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.StumpEditorModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public StumpEditorModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/proxy/{proxyId}/recording/{recordIndex}/newstump"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var recordIndex = (int)_.recordIndex;
                var environment = proxyHost.FindProxy(proxyId);

                var model = new
                {
                    StumpName = "Stump - " + System.Environment.TickCount.ToString(CultureInfo.InvariantCulture),
                    Origin = (int)StumpOrigin.RecordedContext,
                    StumpId = string.Empty,
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName,
                    LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                    BackUrl = "/proxy/" + proxyId + "/recordings",
                    CreateButtonText = "Create New Stump",
                    LoadRecord = true,
                    LoadStump = false,
                    RecordIndex = recordIndex
                };

                return View["stumpeditor", model];
            };

            Get["/proxy/{proxyId}/stumps/{stumpId}"] = _ =>
            {
                var proxyId = (string)_.proxyId;
                var stumpId = (string)_.stumpId;
                var environment = proxyHost.FindProxy(proxyId);
                var stump = environment.Stumps.FindStump(stumpId);

                var model = new
                {
                    StumpName = stump.Contract.StumpName,
                    Origin = (int)StumpOrigin.ExistingStump,
                    StumpId = stump.Contract.StumpId,
                    ProxyId = environment.ProxyId,
                    ExternalHostName = environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName,
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