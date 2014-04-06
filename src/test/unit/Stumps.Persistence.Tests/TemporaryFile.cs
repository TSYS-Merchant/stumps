namespace Stumps
{

    using System;
    using System.IO;

    /// <summary>
    ///     A class used to create a temporary file, and remove it if necessary.
    /// </summary>
    public class TemporaryFile : TemporaryDirectory
    {

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.TemporaryFile"/> class.
        /// </summary>
        public TemporaryFile()
        {
            this.FileName = new Guid().ToString("D");
        }

        /// <summary>
        ///     Gets the name of the temporary file.
        /// </summary>
        /// <value>
        ///     The name of the temporary file.
        /// </value>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the temporary file name and path.
        /// </summary>
        /// <value>
        /// The temporary file and path.
        /// </value>
        public string FileAndPath
        {
            get { return System.IO.Path.Combine(this.Path, this.FileName); }
        }

        /// <summary>
        ///     Ensures the file exists.
        /// </summary>
        public void EnsureExists()
        {
            File.WriteAllText(this.FileAndPath, string.Empty);
        }

        /// <summary>
        ///     Ensures the exists with junk.
        /// </summary>
        public void EnsureExistsWithJunk()
        {
            File.WriteAllText(this.FileAndPath, new Guid().ToString("D"));
        }

    }

}
