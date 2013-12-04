namespace Stumps.Web.ViewModules {

    using Stumps.Proxy;

    public static class ModuleHelper {

        public static string StateValue(ProxyEnvironment environment, string running, string stopped, string recording) {

            var value = stopped;

            if ( environment.IsRunning && environment.RecordTraffic ) {
                value = recording;
            }
            else if ( environment.IsRunning && !environment.RecordTraffic ) {
                value = running;
            }

            return value;

        }

    }

}
