namespace Stumps.Web.ViewModules {

    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Proxy;

    public class MainModule : NancyModule {

        public MainModule(IProxyHost proxyHost) {

            Get["/"] = _ => {

                var environmentList = proxyHost.FindAll();
                var list = new ArrayList();

                foreach ( var environment in environmentList ) {
                    list.Add(new {
                        State = ModuleHelper.StateValue(environment, "running", "stopped", "recording"),
                        StateImage = ModuleHelper.StateValue(environment, "svr_run.png", "svr_stp.png", "svr_rec.png"),
                        ExternalHostName = (environment.UseSsl ? environment.ExternalHostName + " (SSL)" : environment.ExternalHostName),
                        RequestsServed = PrettyNumber(environment.RequestsServed),
                        StumpsServed = PrettyNumber(environment.StumpsServed),
                        LocalWebsite = "http://localhost:" + environment.Port.ToString(CultureInfo.InvariantCulture) + "/",
                        ProxyId = environment.ProxyId,
                        IsRunning = (environment.IsRunning ? "isRunning" : string.Empty),
                        IsRecording = (environment.RecordTraffic ? "isRecording" : string.Empty),
                        RecordingCount = PrettyNumber(environment.Recordings.Count),
                        StumpsCount = PrettyNumber(environment.Stumps.Count)
                    });

                }

                var model = new { Websites = list };
                return View["main", model];

            };

        }

        private static string PrettyNumber(int value) {

            var s = value.ToString("#,#", CultureInfo.InvariantCulture);
            
            if ( string.IsNullOrWhiteSpace(s) ) {
                s = "0";
            }

            return s;

        }

    }

}
