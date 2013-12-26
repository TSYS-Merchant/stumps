namespace Stumps.Web.ViewModules {

    using System;
    using Stumps.Proxy;

    internal static class ModuleHelper {

        public static string StateValue(ProxyEnvironment environment, string running, string stopped, string recording) {

            if ( environment == null ) {
                throw new ArgumentNullException("environment");
            }

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
