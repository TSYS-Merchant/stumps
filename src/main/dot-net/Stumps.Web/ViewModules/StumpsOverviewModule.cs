namespace Stumps.Web.ViewModules
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support the Stumps overview webpage of the Stumps website.
    /// </summary>
    public class StumpsOverviewModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsOverviewModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public StumpsOverviewModule(IStumpsHost stumpsHost)
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/proxy/{serverId}/stumps"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var stumpModelArray = new ArrayList();

                var stumpContractList = server.FindAllContracts();
                stumpContractList = stumpContractList.OrderBy(x => x.StumpName).ToList();

                foreach (var contract in stumpContractList)
                {
                    var stumpModel = new
                    {
                        StumpId = contract.StumpId,
                        StumpName = contract.StumpName
                    };

                    stumpModelArray.Add(stumpModel);
                }

                var model = new
                {
                    ProxyId = server.ServerId,
                    ExternalHostName = server.UseSsl ? $"{server.RemoteServerHostName} (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = $"http://localhost:{server.ListeningPort}/",
                    Stumps = stumpModelArray
                };

                return View["stumpsoverview", model];
            };
        }
    }
}