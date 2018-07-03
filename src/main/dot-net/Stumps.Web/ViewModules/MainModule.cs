namespace Stumps.Web.ViewModules
{

    using System;
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support the overview webpage of the Stumps website.
    /// </summary>
    public class MainModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.MainModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="T:Stumps.Server.IStumpsHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public MainModule(IStumpsHost stumpsHost)
        {

            if (stumpsHost == null)
            {
                throw new ArgumentNullException("stumpsHost");
            }

            Get["/"] = _ =>
            {

                var servers = stumpsHost.FindAll();
                servers = servers.OrderBy(x => x.RemoteServerHostName).ToList();

                var list = new ArrayList();

                var hostName = ResolveMachineName();

                foreach (var server in servers)
                {
                    var schema = server.UseHttpsForIncomingConnections ? "https" : "http";

                    list.Add(
                        new
                        {
                            State = ModuleHelper.StateValue(server, "running", "stopped", "recording"),
                            StateImage = ModuleHelper.StateValue(server, "svr_run.png", "svr_stp.png", "svr_rec.png"),
                            ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                            RequestsServed = PrettyNumber(server.TotalRequestsServed),
                            StumpsServed = PrettyNumber(server.RequestsServedWithStump),
                            LocalWebsite = string.Format("{0}://{1}:{2}/", schema, hostName, server.ListeningPort),
                            ProxyId = server.ServerId,
                            IsRunning = server.IsRunning ? "isRunning" : string.Empty,
                            IsRecording = server.RecordTraffic ? "isRecording" : string.Empty,
                            RecordingCount = PrettyNumber(server.Recordings.Count),
                            StumpsCount = PrettyNumber(server.StumpCount)
                        });

                }

                var model = new
                {
                    Websites = list
                };
                return View["main", model];

            };

        }

        /// <summary>
        ///     Transforms a number into a <see cref="T:System.String"/> that uses a comma as the thousands separator.
        /// </summary>
        /// <param name="value">The value to convert to a <see cref="T:System.String"/>.</param>
        /// <returns>A <see cref="T:System.String"/> representing the formatted form of <paramref name="value"/>.</returns>
        private static string PrettyNumber(int value)
        {

            var s = value.ToString("#,#", CultureInfo.InvariantCulture);

            if (string.IsNullOrWhiteSpace(s))
            {
                s = "0";
            }

            return s;

        }

        private static string ResolveMachineName()
        {
            var hostName = Dns.GetHostName();
            var domainName = IPGlobalProperties.GetIPGlobalProperties().DomainName;

            if (!string.IsNullOrWhiteSpace(domainName) &&
                !hostName.EndsWith(domainName, StringComparison.OrdinalIgnoreCase))
            {
                hostName += "." + domainName;
            }

            return hostName;
        }

    }

}