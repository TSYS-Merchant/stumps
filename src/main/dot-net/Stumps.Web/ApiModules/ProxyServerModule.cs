namespace Stumps.Web.ApiModules
{

    using System;
    using System.Collections.Generic;
    using Nancy;
    using Nancy.ModelBinding;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the managing proxy servers through the REST API.
    /// </summary>
    public sealed class ProxyServerModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ApiModules.ProxyServerModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public ProxyServerModule(IStumpsHost stumpsHost) : base("/api")
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/proxy"] = _ =>
            {

                var modelList = new List<ProxyServerDetailsModel>();
                var serverList = stumpsHost.FindAll();

                foreach (var server in serverList)
                {
                    var model = new ProxyServerDetailsModel
                    {
                        AutoStart = server.AutoStart,
                        ExternalHostName = server.ExternalHostName,
                        IsRunning = server.IsRunning,
                        Port = server.ListeningPort,
                        RecordCount = server.Recordings.Count,
                        RecordTraffic = server.RecordTraffic,
                        RequestsServed = server.TotalRequestsServed,
                        StumpsCount = server.StumpCount,
                        StumpsServed = server.RequestsServedWithStump,
                        ProxyId = server.ServerId,
                        UseSsl = server.UseSsl
                    };

                    modelList.Add(model);
                }

                return Response.AsJson(modelList);

            };

            Post["/proxy"] = _ =>
            {
                var model = this.Bind<ProxyServerModel>();

                stumpsHost.CreateServerInstance(model.ExternalHostName, model.Port, model.UseSsl, model.AutoStart);

                return HttpStatusCode.Created;
            };

            Delete["/proxy/{serverId}"] = _ =>
            {
                var serverId = (string)_.serverId;

                stumpsHost.DeleteServerInstance(serverId);

                return HttpStatusCode.OK;
            };

        }

    }

}