namespace Stumps.Web.Api
{

    using System;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for the discovering available ports on the server through the REST API.
    /// </summary>
    public class NetworkPortModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.Api.NetworkPortModule"/> class.
        /// </summary>
        /// <param name="host">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="host"/> is <c>null</c>.</exception>
        public NetworkPortModule(IStumpsHost host) : base("/api/local")
        {

            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            Get["/network/port/"] = context => FindAvailablePort(context);
            Get["/network/port/{port}"] = context => FindPortInformation(context);

        }

        /// <summary>
        ///     Finds an available port on the local machine.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response FindAvailablePort(dynamic context)
        {

            var port = NetworkInformation.FindRandomOpenPort();
            var model = new
            {
                port = port,
                portIsAvailable = true
            };

            return Response.AsJson(model);

        }

        /// <summary>
        ///     Finds information about a port specified in the context.
        /// </summary>
        /// <param name="context">The context of the request.</param>
        /// <returns>A <see cref="T:Nancy.Response"/> containing the response to the request.</returns>
        private Response FindPortInformation(dynamic context)
        {

            var model = new
            {
                port = context.port,
                portIsAvailable = !NetworkInformation.IsPortBeingUsed(context.port)
            };

            return Response.AsJson(model);

        }

    }

}