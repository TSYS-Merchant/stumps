namespace Stumps.Web.ViewModules
{

    using System;
    using System.Globalization;
    using Nancy;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for creating or editing Stumps.
    /// </summary>
    public class StumpEditorModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.StumpEditorModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public StumpEditorModule(IStumpsHost stumpsHost)
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/proxy/{serverId}/recording/{recordIndex}/newstump"] = _ =>
            {
                var serverId = (string)_.serverId;
                var recordIndex = (int)_.recordIndex;
                var server = stumpsHost.FindServer(serverId);

                var model = new
                {
                    StumpName = "Stump - " + System.Environment.TickCount.ToString(CultureInfo.InvariantCulture),
                    Origin = (int)StumpOrigin.RecordedContext,
                    StumpId = string.Empty,
                    ProxyId = server.ServerId,
                    ServerName = server.ServerName,
                    ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = "http://localhost:" + server.ListeningPort.ToString(CultureInfo.InvariantCulture) + "/",
                    BackUrl = "/proxy/" + serverId + "/recordings",
                    CreateButtonText = "Create New Stump",
                    LoadRecord = true,
                    LoadStump = false,
                    RecordIndex = recordIndex
                };

                return View["stumpeditor", model];
            };

            Get["/proxy/{serverId}/stumps/{stumpId}"] = _ =>
            {
                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = stumpsHost.FindServer(serverId);
                var stump = server.FindStump(stumpId);

                var model = new
                {
                    StumpName = stump.StumpName,
                    Origin = (int)StumpOrigin.ExistingStump,
                    StumpId = stump.StumpId,
                    ProxyId = server.ServerId,
                    ServerName = server.ServerName,
                    ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = "http://localhost:" + server.ListeningPort.ToString(CultureInfo.InvariantCulture) + "/",
                    BackUrl = "/proxy/" + serverId + "/stumps",
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