namespace Stumps {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {

            IMessageWriter writer;
            IStartup startup;

            var isConsole = Environment.UserInteractive;

            if ( isConsole ) {
                writer = new ConsoleWriter();
                startup = new ConsoleStartup(writer);
            }
            else {
                writer = new EventLogWriter();
                startup = new ServiceStartup();
            }

            if ( IsApplicationAlreadyRunning() ) {
                writer.WriteError(Resources.ApplicationRunning);
                return;
            }

            startup.RunInstance(args);

        }

        private static bool IsApplicationAlreadyRunning() {

            var applicationName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            var processes = Process.GetProcessesByName(applicationName);

            foreach ( var process in processes ) {
                process.Dispose();
            }

            var isAlreadyRunning = processes.Length > 1;

            return isAlreadyRunning;

        }

    }

}
