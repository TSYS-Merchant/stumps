namespace Stumps {

    using System;

    public class ConsoleStartup : IStartup {

        private readonly IMessageWriter _messageWriter;

        public ConsoleStartup(IMessageWriter messageWriter) {
            _messageWriter = messageWriter;
        }

        public void RunInstance(string[] args) {

            args = args ?? new string[] { };
            var startupAction = determineStartupByCommandLineArguments(args);
            startupAction.Invoke();

        }

        private Action determineStartupByCommandLineArguments(string[] args) {

            var hasInstall = false;
            var hasUninstall = false;

            var hasArguments = (args.Length > 0);

            for ( int i = 0; i < args.Length; i++ ) {
                if ( args[i].Equals(Resources.InstallCommandLineArg, StringComparison.OrdinalIgnoreCase) ) {
                    hasInstall = true;
                }
                else if ( args[i].Equals(Resources.UninstallCommandLineArg, StringComparison.OrdinalIgnoreCase) ) {
                    hasUninstall = true;
                }
            }

            if ( hasInstall && hasUninstall ) {
                hasInstall = false;
                hasUninstall = false;
            }

            var showHelp = (!hasInstall && !hasUninstall && hasArguments);

            var executeAction = new Action(runStumpsAction);

            if ( hasInstall ) {
                executeAction = new Action(installServiceAction);
            }
            else if ( hasUninstall ) {
                executeAction = new Action(uninstallServiceAction);
            }
            else if ( showHelp ) {
                executeAction = new Action(showHelpAction);
            }

            return executeAction;

        }

        private void installServiceAction() {

            _messageWriter.WriteMessage(Resources.InstallStarting);

            var installHelper = new ServiceInstallHelper();
            var installSuccess = installHelper.InstallService();

            if ( installSuccess ) {
                _messageWriter.WriteMessage(Resources.InstallComplete);
            }
            else {
                _messageWriter.WriteMessage(Resources.InstallFailed);
            }

        }

        private void runStumpsAction() {

            _messageWriter.WriteMessage(Resources.StartupStarting);

            using ( var server = new StumpsServer() ) {
                server.Start();
                _messageWriter.WriteMessage(Resources.StartupComplete);

                Console.ReadLine();

                _messageWriter.WriteMessage(Resources.ShutdownStarting);
                server.Stop();
                _messageWriter.WriteMessage(Resources.ShutdownComplete);
            }

        }

        private void showHelpAction() {
            _messageWriter.WriteMessage(Resources.HelpInformation);
        }

        private void uninstallServiceAction() {

            _messageWriter.WriteMessage(Resources.UninstallStarting);

            var installHelper = new ServiceInstallHelper();
            var uninstallSuccess = installHelper.UninstallService();

            if ( uninstallSuccess ) {
                _messageWriter.WriteMessage(Resources.UninstallComplete);
            }
            else {
                _messageWriter.WriteMessage(Resources.UninstallFailed);
            }

        }

    }

}
