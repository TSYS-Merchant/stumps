namespace Stumps.Web.ViewModules
{

    using System;
    using Nancy;
    using Stumps.Proxy;
    using Stumps.Utility;

    public class AddWebsiteModule : NancyModule
    {

        public AddWebsiteModule(IProxyHost host)
        {

            Get["/AddWebsite"] = _ =>
            {
                var port = NetworkUtility.FindRandomOpenPort();

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
                    host.CreateProxy(hostNameTextBox, port, useSsl, true);
                }

                return Response.AsRedirect("/");

            };

        }

    }

}