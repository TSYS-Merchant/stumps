namespace Stumps.Web.Api
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Stumps.Server;

    public sealed class ServerStatusModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.Api.ServerStatusModule"/> class.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="host"/> is <c>null</c>.</exception>
        public ServerStatusModule(IStumpsHost host) : base("/api/local")
        {

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            Get["/servers/status"] = context => FindServersStatus(host, context);
            Get["/servers/{serverId}/status"] = context => FindServerStatus(host, context);

        }

        /// <summary>
        ///     Finds the status of a specified server.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        public Response FindServerStatus(IStumpsHost host, dynamic context)
        {

            var serverId = (string)context.serverId;
            var server = host.FindServer(serverId);

            var model = new ServerStatusModel
            {
                IsRecording = server.RecordTraffic,
                IsRunning = server.IsRunning,
                RecordedRequests = server.Recordings.Count,
                RequestsHandledByRemoteServer = server.RequestsServedByRemoteServer,
                RequestsHandledByServer = server.TotalRequestsServed,
                RequestsHandledByStump = server.RequestsServedWithStump,
                ServerId = server.ServerId,
                StumpsInServer = server.StumpCount
            };

            return Response.AsJson(model);

        }

        /// <summary>
        ///     Finds the status of all servers.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        public Response FindServersStatus(IStumpsHost host, dynamic context)
        {

            var modelList = new List<ServerModel>();
            var serverList = host.FindAll();

            foreach (var server in serverList)
            {
                var model = new ServerModel
                {
                    AutoStart = server.AutoStart,
                    FallbackResponse = FallbackResponse.Undefined,
                    ListeningPort = server.ListeningPort,
                    RemoteServerHostName = server.RemoteServerHostName,
                    RemoteServerRequiresSsl = server.UseSsl,
                    ServerId = server.ServerId,
                    UseRemoteServerForRequests = true,
                    UseStumpsForRequests = true,
                };

                modelList.Add(model);
            }

            return Response.AsJson(modelList);

        }

    }

}
