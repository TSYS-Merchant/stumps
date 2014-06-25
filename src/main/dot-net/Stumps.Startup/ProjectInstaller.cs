namespace Stumps
{

    using System.ComponentModel;

    /// <summary>
    ///     A class that provides the installation capbilities for the application to run as a Windows Service.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.ProjectInstaller"/> class.
        /// </summary>
        public ProjectInstaller()
        {
            InitializeComponent();
        }

    }

}