namespace Stumps.Web.ApiModules
{
    using System;
    using Nancy;
    using Stumps.Server;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides support for the discovering available ports on the server through the REST API.
    /// </summary>
    public class PortAvailableModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PortAvailableModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public PortAvailableModule(IStumpsHost stumpsHost) : base("/api")
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/portAvailable/{port}"] = _ =>
            {
                var port = _.port;

                var model = new PortAvailableModel
                {
                    PortAvailable = !NetworkInformation.IsPortBeingUsed(port),
                };

                return Response.AsJson(model);
            };
        }
    }
}