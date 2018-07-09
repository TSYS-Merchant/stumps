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
        ///     Initializes a new instance of the <see cref="StumpEditorModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public StumpEditorModule(IStumpsHost stumpsHost)
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/proxy/{serverId}/recording/{recordIndex}/newstump"] = _ =>
            {
                var serverId = (string)_.serverId;
                var recordIndex = (int)_.recordIndex;
                var server = stumpsHost.FindServer(serverId);

                var model = new
                {
                    StumpName = $"Stump - {Environment.TickCount}",
                    Origin = (int)StumpOrigin.RecordedContext,
                    StumpId = string.Empty,
                    ProxyId = server.ServerId,
                    ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = $"http://localhost:{server.ListeningPort}/",
                    BackUrl = $"/proxy/{serverId}/recordings",
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
                    ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = $"http://localhost:{server.ListeningPort}/",
                    BackUrl = $"/proxy/{serverId}/stumps",
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