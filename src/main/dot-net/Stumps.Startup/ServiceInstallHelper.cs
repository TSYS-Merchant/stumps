namespace Stumps {

    using System;
    using System.Reflection;
    using System.ServiceProcess;
    using System.Configuration.Install;

    public class ServiceInstallHelper {

        public bool InstallService() {

            var result = false;

            if ( !isServiceInstalled() ) {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                result = true;
            }

            return result;

        }

        public bool UninstallService() {

            var result = false;

            if ( isServiceInstalled() ) {
                ManagedInstallerClass.InstallHelper(new string[] { @"/u", Assembly.GetExecutingAssembly().Location });
                result = true;
            }

            return result;

        }

        private static bool isServiceInstalled() {

            var isInstalled = false;
            var services = ServiceController.GetServices();

            foreach ( var service in services ) {
                if ( service.ServiceName.Equals(Resources.ServiceName, StringComparison.OrdinalIgnoreCase) ) {
                    isInstalled = true;
                }

                service.Dispose();
            }

            return isInstalled;

        }

    }

}
