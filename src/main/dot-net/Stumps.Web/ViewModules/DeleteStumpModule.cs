namespace Stumps.Web.ViewModules
{
    using System;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for deleting a Stump from a proxy sever through the Stumps website.
    /// </summary>
    public class DeleteStumpModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteStumpModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public DeleteStumpModule(IStumpsHost stumpsHost)
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/proxy/{serverId}/stumps/{stumpId}/delete"] = _ =>
            {
                var serverId = (string)_.serverId;
                var stumpId = (string)_.stumpId;
                var server = stumpsHost.FindServer(serverId);
                var stump = server.FindStump(stumpId);

                var model = new
                {
                    StumpName = stump.StumpName,
                    StumpId = stump.StumpId,
                    ProxyId = server.ServerId
                };

                return View["deletestump", model];
            };
        }
    }
}