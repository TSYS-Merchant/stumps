namespace Stumps
{

    using System;
    using System.IO;

    /// <summary>
    ///     A class that creates a temporary directory and cleans it up when its no longer needed.
    /// </summary>
    public class TemporaryDirectory : IDisposable
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.TemporaryDirectory"/> class.
        /// </summary>
        public TemporaryDirectory()
        {
            var temporaryName = new Guid().ToString("D");
            this.Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), temporaryName);
            Directory.CreateDirectory(this.Path);
        }

        /// <summary>
        ///     Gets the path to the temporary directory.
        /// </summary>
        /// <value>
        ///     The path to the temporary directory.
        /// </value>
        public string Path
        {
            get;
            private set;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

            if (Directory.Exists(this.Path))
            {
                Directory.Delete(this.Path, true);
            }

        }

    }

}
