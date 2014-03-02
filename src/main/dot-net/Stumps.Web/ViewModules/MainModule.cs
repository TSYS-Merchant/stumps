namespace Stumps.Web.ViewModules
{

    using System;
    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    /// <summary>
    ///     A class that provides support the overview webpage of the Stumps website.
    /// </summary>
    public class MainModule : NancyModule
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Web.ViewModules.MainModule"/> class.
        /// </summary>
        /// <param name="proxyHost">The <see cref="T:Stumps.Proxy.IProxyHost"/> used by the instance.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="proxyHost"/> is <c>null</c>.</exception>
        public MainModule(IProxyHost proxyHost)
        {

            if (proxyHost == null)
            {
                throw new ArgumentNullException("proxyHost");
            }

            Get["/"] = _ =>
            {

                var environmentList = proxyHost.FindAll();
                var list = new ArrayList();

                foreach (var environment in environmentList)
                {
                    list.Add(
                        new
                        {
                            State = ModuleHelper.StateValue(environment, "running", "stopped", "recording"),
                            StateImage = ModuleHelper.StateValue(environment, "svr_run.png", "svr_stp.png", "svr_rec.png"),
                            ExternalHostName = environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName,
                            RequestsServed = PrettyNumber(environment.RequestsServed),
                            StumpsServed = PrettyNumber(environment.StumpsServed),
                            LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                            ProxyId = environment.ProxyId,
                            IsRunning = environment.IsRunning ? "isRunning" : string.Empty,
                            IsRecording = environment.RecordTraffic ? "isRecording" : string.Empty,
                            RecordingCount = PrettyNumber(environment.Recordings.Count),
                            StumpsCount = PrettyNumber(environment.Stumps.Count)
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

    }

}