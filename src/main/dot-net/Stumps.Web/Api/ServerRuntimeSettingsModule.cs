namespace Stumps.Web.Api
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for altering the runtime settings of a Stumps server. 
    /// </summary>
    public sealed class ServerRuntimeSettingsModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.Api.ServerModule"/> class.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="host"/> is <c>null</c>.</exception>
        public ServerRuntimeSettingsModule(IStumpsHost host) : base("/api/local")
        {

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            Put["/servers/{serverId}/record"] = context => ChangeRecordTraffic(host, context, true);
            Delete["/servers/{serverId}/record"] = context => ChangeRecordTraffic(host, context, false);

        }

        /// <summary>
        ///     Creates a new Stumps server instance.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <param name="context">The context of the request.</param>
        /// <param name="recordTraffic"><c>true</c> to record incomming requests; otherwise, <c>false</c>.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response ChangeRecordTraffic(IStumpsHost host, dynamic context, bool recordTraffic)
        {

            var model = this.Bind<ServerModel>();

            host.CreateServerInstance(model.RemoteServerHostName, model.ListeningPort, model.RemoteServerRequiresSsl, model.AutoStart);

            return new Response()
            {
                StatusCode = HttpStatusCode.Created
            };

        }

    }

}