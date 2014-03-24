namespace Stumps.Launcher
{

    using System;
    using System.Diagnostics;
    using System.IO;
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
        [STAThread]
        public static void Main()
        {

            var configurationFile = Path.Combine(
                DefaultConfigurationSettings.StoragePath, DefaultConfigurationSettings.ConfigurationFileName);

            var configurationDal = new ConfigurationDataAccess(configurationFile);
            var configuration = new StumpsConfiguration(configurationDal);
            configurationDal.EnsureConfigurationIsInitialized(configuration.SaveConfiguration);

            configuration.LoadConfiguration();

            var urlString = string.Format("http://localhost:{0}/", configuration.WebApiPort);

            Process.Start(urlString);

        }

    }

}
