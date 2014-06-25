namespace Stumps
{

    using Stumps.Server;

    /// <summary>
    ///     An interface used to abstract the starting environment of a Stumps server.
    /// </summary>
    public interface IStartup
    {

        /// <summary>
        ///     Gets or sets the configuration for the Stumps server.
        /// </summary>
        /// <value>
        ///     The configuration for the Stumps server.
        /// </value>
        StumpsConfiguration Configuration { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="T:Stumps.IMessageWriter"/> used to record startup messages.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IMessageWriter"/> used to record startup messages.
        /// </value>
        IMessageWriter MessageWriter { get; set; }

        /// <summary>
        ///     Runs the instance of the Stumps server.
        /// </summary>
        void RunInstance();

    }

}