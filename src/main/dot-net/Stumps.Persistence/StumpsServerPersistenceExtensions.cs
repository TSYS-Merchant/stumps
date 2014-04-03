namespace Stumps
{

    /// <summary>
    ///     A class that provides a set of extension methods to the <see cref="T:Stumps.StumpsServer"/> used 
    ///     to preserve and retrieve the state of <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    public static class StumpsServerPersistenceExtensions
    {

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump"/> to the server persisted in an archive.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> the <see cref="T:Stumps.Stump"/> is added to.</param>
        /// <param name="fileName">The path and file name for the archive containing the persisted <see cref="T:Stumps.Stump"/>.</param>
        /// <returns>The <see cref="T:Stumps.Stump"/> added to the server from the specified <paramref name="fileName"/>.</returns>
        public static Stump AddStumpFromArchive(this StumpsServer server, string fileName)
        {

            return server.AddStumpFromArchive(fileName, null);

        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump"/> to the server persisted in an archive.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> the <see cref="T:Stumps.Stump"/> is added to.</param>
        /// <param name="fileName">The path and file name for the archive containing the persisted <see cref="T:Stumps.Stump"/>.</param>
        /// <param name="newStumpId">The new identifier given to the <see cref="Stumps.Stump"/>.</param>
        /// <returns>The <see cref="T:Stumps.Stump"/> added to the server from the specified <paramref name="fileName"/>.</returns>
        public static Stump AddStumpFromArchive(this StumpsServer server, string fileName, string newStumpId)
        {

            return null;

        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump"/> to the server persisted in a specified directory.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> the <see cref="T:Stumps.Stump"/> is added to.</param>
        /// <param name="path">The path to the directory that contains the persisted <see cref="T:Stumps.Stump"/>.</param>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump"/> located at the specified <paramref name="path"/>.</param>
        /// <returns>The <see cref="T:Stumps.Stump"/> added to the server from the specified <paramref name="path"/>.</returns>
        public static Stump AddStumpFromDirectory(this StumpsServer server, string path, string stumpId)
        {

            return server.AddStumpFromDirectory(path, stumpId, null);

        }

        /// <summary>
        ///     Adds a new <see cref="T:Stumps.Stump"/> to the server persisted in a specified directory.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> the <see cref="T:Stumps.Stump"/> is added to.</param>
        /// <param name="path">The path to the directory that contains the persisted <see cref="T:Stumps.Stump"/>.</param>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump"/> located at the specified <paramref name="path"/>.</param>
        /// <param name="newStumpId">The new identifier given to the <see cref="Stumps.Stump"/>.</param>
        /// <returns>The <see cref="T:Stumps.Stump"/> added to the server from the specified <paramref name="path"/>.</returns>
        public static Stump AddStumpFromDirectory(this StumpsServer server, string path, string stumpId, string newStumpId)
        {

            return null;

        }

        /// <summary>
        ///     Persists a specified <see cref="T:Stumps.Stump"/> to an archive file.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> containing the <see cref="T:Stumps.Stump"/>.</param>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump"/> to archive.</param>
        /// <param name="fileName">The name of the archive file.</param>
        public static void SaveStumpToArchive(this StumpsServer server, string stumpId, string fileName)
        {

            var stump = server.FindStump(stumpId);
            stump.SaveToArchive(fileName);

        }

        /// <summary>
        ///     Persists a specified <see cref="T:Stumps.Stump"/> to a directory.
        /// </summary>
        /// <param name="server">The <see cref="T:Stumps.StumpsServer"/> containing the <see cref="T:Stumps.Stump"/>.</param>
        /// <param name="stumpId">The unique identifier for the <see cref="T:Stumps.Stump"/> to save.</param>
        /// <param name="path">The path to the directory where the <see cref="T:Stumps.Stump"/> is saved.</param>
        public static void SaveStumpToDirectory(this StumpsServer server, string stumpId, string path)
        {

            var stump = server.FindStump(stumpId);
            stump.SaveToDirectory(path);

        }

    }

}
