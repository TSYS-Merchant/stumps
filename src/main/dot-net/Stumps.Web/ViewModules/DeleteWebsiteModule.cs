namespace Stumps.Web.ViewModules
{

    using System;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for deleting a proxy server through the Stumps website.
    /// </summary>
    public class DeleteWebsiteModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.DeleteWebsiteModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public DeleteWebsiteModule(IStumpsHost stumpsHost)
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/proxy/{serverId}/delete"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var model = new
                {
                    ProxyId = server.ServerId,
                    ServerName = server.ServerName,
                    ExternalHostName = server.RemoteServerHostName,
                };

                return View["deletewebsite", model];
            };

        }

    }

}