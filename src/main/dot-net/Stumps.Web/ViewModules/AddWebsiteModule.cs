namespace Stumps.Web.ViewModules
{
    using System;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for adding a new proxy server through the Stumps website.
    /// </summary>
    public class AddWebsiteModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.AddWebsiteModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public AddWebsiteModule(IStumpsHost stumpsHost)
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/AddWebsite"] = _ =>
            {
                var port = NetworkInformation.FindRandomOpenPort();

                var model = new
                {
                    OpenPort = port
                };

                return View["addwebsite", model];
            };

            Post["/AddWebsite"] = _ =>
            {
                var hostNameTextBox = ((string)(Request.Form.hostNameTextBox.Value ?? string.Empty)).Trim();
                var portTextBox = ((string)(Request.Form.portTextBox.Value ?? string.Empty)).Trim();
                var useSslCheckBox = ((string)(Request.Form.useSslCheckBox.Value ?? "off")).Trim();

                int port;
                port = int.TryParse(portTextBox, out port) ? port : 0;
                var useSsl = useSslCheckBox.Equals("on", StringComparison.OrdinalIgnoreCase);

                if (!string.IsNullOrEmpty(hostNameTextBox) && port > 0)
                {
                    stumpsHost.CreateServerInstance(hostNameTextBox, port, useSsl, true);
                }

                return Response.AsRedirect("/");
            };
        }
    }
}