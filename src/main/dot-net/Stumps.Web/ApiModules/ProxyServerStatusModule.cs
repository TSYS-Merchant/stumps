namespace Stumps.Web.ApiModules
{

    using System;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the discovering status of a proxy server through the REST API.
    /// </summary>
    public class ProxyServerStatusModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.ProxyServerStatusModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public ProxyServerStatusModule(IStumpsHost stumpsHost)
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/api/proxy/{serverId}/status"] = _ =>
            {

                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var model = new RunningStatusModel
                {
                    IsRunning = server.IsRunning
                };

                return Response.AsJson(model);

            };

            Put["/api/proxy/{serverId}/status"] = _ =>
            {

                var serverId = (string)_.serverId;
                var environment = stumpsHost.FindServer(serverId);

                var model = this.Bind<RunningStatusModel>();

                if (model.IsRunning && !environment.IsRunning)
                {
                    stumpsHost.Start(serverId);
                }
                else if (!model.IsRunning && environment.IsRunning)
                {
                    stumpsHost.Shutdown(serverId);
                }

                return Response.AsJson(model);

            };

        }

    }

}