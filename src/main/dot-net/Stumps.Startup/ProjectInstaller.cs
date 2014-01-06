namespace Stumps {

    using System;
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer {

        public ProjectInstaller() {
            InitializeComponent();
        }

    }

}
