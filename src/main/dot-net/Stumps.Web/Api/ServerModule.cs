namespace Stumps.Web.Api
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for the managing Stumps servers through the REST API.
    /// </summary>
    public sealed class ServerModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.Api.ServerModule"/> class.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="host"/> is <c>null</c>.</exception>
        public ServerModule(IStumpsHost host) : base("/api/local")
        {

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            Get["/servers"] = context => FindAllServers(host, context);
            Post["/servers"] = context => CreateServer(host, context);
            Get["/servers/{serverId}"] = context => FindServer(host, context);
            Delete["/servers/{serverId}"] = context => DeleteServer(host, context);

        }

        /// <summary>
        ///     Creates a new Stumps server instance.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response CreateServer(IStumpsHost host, dynamic context)
        {

            var model = this.Bind<ServerModel>();

            host.CreateServerInstance(model.RemoteServerHostName, model.ListeningPort, model.RemoteServerRequiresSsl, model.AutoStart);

            return new Response()
            {
                StatusCode = HttpStatusCode.Created
            };

        }

        /// <summary>
        ///     Deletes the server specified in the context.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response DeleteServer(IStumpsHost host, dynamic context)
        {

            var serverId = (string)context.serverId;
            host.DeleteServerInstance(serverId);

            return new Response()
            {
                StatusCode = HttpStatusCode.OK
            };

        }

        /// <summary>
        ///     Finds the server specified in the context.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response FindServer(IStumpsHost host, dynamic context)
        {

            var serverId = (string)context.serverId;
            var server = host.FindServer(serverId);

            var model = new ServerModel
            {
                AutoStart = server.AutoStart,
                FallbackResponse = FallbackResponse.Undefined,
                ListeningPort = server.ListeningPort,
                RemoteServerHostName = server.RemoteServerHostName,
                RemoteServerRequiresSsl = server.UseSsl,
                ServerId = server.ServerId,
                UseRemoteServerForRequests = true,
                UseStumpsForRequests = true
            };

            return Response.AsJson(model);

        }

        /// <summary>
        ///     Finds all servers in the host.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response FindAllServers(IStumpsHost host, dynamic context)
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
                    UseStumpsForRequests = true
                };

                modelList.Add(model);
            }

            return Response.AsJson(modelList);

        }

    }

}