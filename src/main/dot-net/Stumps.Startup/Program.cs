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

            var isRunningAsConsole = IsApplicationRunningAsConsole(args);

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

            string configurationFile;
            FindSwitchAndValue("-c", args, out configurationFile);

            if (configurationFile != null && !File.Exists(configurationFile))
            {
                return null;
            }

            return configurationFile;

        }

        /// <summary>
        ///     Finds the switch passed into the command-line arguments.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="args">The command-line arguments.</param>
        /// <returns><c>true</c> if the switch was found; otherwise, <c>false</c>.</returns>
        private static bool FindSwitch(string switchName, string[] args)
        {

            string valueAfter;
            return FindSwitchAndValue(switchName, args, out valueAfter);

        }

        /// <summary>
        ///     Finds the switch passed into the command-line arguments.
        /// </summary>
        /// <param name="switchName">Name of the switch.</param>
        /// <param name="args">The command-line arguments.</param>
        /// <param name="value">The value that was found immediately after the switch.</param>
        /// <returns><c>true</c> if the switch was found; otherwise, <c>false</c>.</returns>
        private static bool FindSwitchAndValue(string switchName, string[] args, out string value)
        {

            var hasSwitch = false;

            value = null;

            for (var i = 0; i < args.Length; i++)
            {
                var argument = args[i];

                if (argument.Equals(switchName, StringComparison.OrdinalIgnoreCase))
                {
                    hasSwitch = true;

                    // Find the value after the switch if one is available
                    if (i <= args.Length - 2)
                    {
                        value = args[i + 1];
                    }
                }
            }

            return hasSwitch;

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


        /// <summary>
        ///     Determines whether the current application is running as a console.
        /// </summary>
        /// <param name="args">The command-line arguments passed into the application.</param>
        /// <returns><c>true</c> if the application is running as a console; otherwise, <c>false</c>.</returns>
        private static bool IsApplicationRunningAsConsole(string[] args)
        {

            // This works on Windows platforms
            if (Environment.UserInteractive)
            {
                return true;
            }

            bool isRunningAsConsole = Environment.UserInteractive;

            // Detect if mono is running
            if (Type.GetType("Mono.Runtime") != null)
            {
                isRunningAsConsole = !FindSwitch("-s", args);
            }

            return isRunningAsConsole;

        }

    }

}