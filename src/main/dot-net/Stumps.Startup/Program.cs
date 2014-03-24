namespace Stumps
{

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using Stumps.Server;
    using Stumps.Server.Data;

    /// <summary>
    ///     A class that provides the primary entry point for the application.
    /// </summary>
    public static class Program
    {

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Stumps.IMessageWriter.WriteError(System.String)", Justification = "The literal is just an empty space.")]
        public static void Main(string[] args)
        {

            var isRunningAsConsole = Environment.UserInteractive;

            var writer = isRunningAsConsole
                              ? (IMessageWriter)new ConsoleWriter()
                              : (IMessageWriter)new EventLogWriter();

            if (IsApplicationAlreadyRunning())
            {
                writer.WriteError(StartupResources.ApplicationRunning);
                return;
            }

            var configurationFile = Path.Combine(
                DefaultConfigurationSettings.StoragePath, DefaultConfigurationSettings.ConfigurationFileName);

            if (args != null && args.Length > 0)
            {
                configurationFile = DetermineConfigurationFileFromArgs(args);
            }

            if (configurationFile == null)
            {

                writer.WriteError(StartupResources.InvalidArguments + string.Join(@" ", args));

                if (isRunningAsConsole)
                {
                    writer.Information(StartupResources.HelpInformation);
                }

                return;

            }

            var configurationDal = new ConfigurationDataAccess(configurationFile);
            var configuration = new StumpsConfiguration(configurationDal);
            configurationDal.EnsureConfigurationIsInitialized(configuration.SaveConfiguration);

            configuration.LoadConfiguration();

            var startup = isRunningAsConsole ? (IStartup)new ConsoleStartup() : (IStartup)new ServiceStartup();

            startup.Configuration = configuration;
            startup.MessageWriter = writer;

            startup.RunInstance();

        }

        /// <summary>
        ///     Determines the configuration file to use based on the command-line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>A <see cref="T:System.String"/> containing the file name for the configuration file.</returns>
        private static string DetermineConfigurationFileFromArgs(string[] args)
        {

            if (!args[0].Equals("-c", StringComparison.OrdinalIgnoreCase) || args.Length != 2)
            {
                return null;
            }

            if (!File.Exists(args[1]))
            {
                return null;
            }

            return args[1];

        }

        /// <summary>
        ///     Determines whether the application is already running.
        /// </summary>
        /// <returns><c>true</c> if the application is running; otherwise, <c>false</c>.</returns>
        private static bool IsApplicationAlreadyRunning()
        {

            var applicationName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location);
            var processes = Process.GetProcessesByName(applicationName);

            foreach (var process in processes)
            {
                process.Dispose();
            }

            var isAlreadyRunning = processes.Length > 1;

            return isAlreadyRunning;

        }

    }

}