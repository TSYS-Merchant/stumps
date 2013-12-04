namespace Stumps.Logging {

    using System;
    using System.Diagnostics;

    public class DebugLogger : ILogger {

        public void LogInfo(string data) {

            if ( data != null ) {
                Debug.WriteLine(Resources.LogInformationPrefix + data);
            }

        }

        public void LogException(string location, Exception exception) {

            if ( location != null && exception != null ) {
                Debug.WriteLine(Resources.LogExceptionPrefix + "(" + location + ") " + exception.ToString());
            }

        }

    }

}
