namespace Stumps {

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Stumps.Data;

    public static class Program {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args) {

            var isRunningAsConsole = Environment.UserInteractive;

            var writer = (isRunningAsConsole ? (IMessageWriter)new ConsoleWriter() : (IMessageWriter) new EventLogWriter());

            if ( IsApplicationAlreadyRunning() ) {
                writer.Error(Resources.ApplicationRunning);
                return;
            }

            var configurationFile = Path.Combine(DefaultConfigurationSettings.StoragePath,
                                                 DefaultConfigurationSettings.ConfigurationFileName);

            if ( args != null && args.Length > 0 ) {
                configurationFile = DetermineConfigurationFileFromArgs(args);
            }

            if ( configurationFile == null ) {

                writer.Error(Resources.InvalidArguments + String.Join(" ", args));

                if ( isRunningAsConsole ) {
                    writer.Information(Resources.HelpInformation);
                }

                return;

            }

            var configurationDal = new ConfigurationDataAccess(configurationFile);
            var configuration = new Configuration(configurationDal);

            if ( !File.Exists(configurationFile) ) {
                configuration.SaveConfiguration();
            }

            configuration.LoadConfiguration();

            var startup = ( isRunningAsConsole ? (IStartup) new ConsoleStartup() : (IStartup) new ServiceStartup() );
            
            startup.Configuration = configuration;
            startup.MessageWriter = writer;

            startup.RunInstance();

        }

        private static string DetermineConfigurationFileFromArgs(string[] args) {

            if (!args[0].Equals("-c", StringComparison.OrdinalIgnoreCase) || args.Length != 2) {
                return null;
            }

            if (!File.Exists(args[1])) {
                return null;
            }

            return args[1];

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
