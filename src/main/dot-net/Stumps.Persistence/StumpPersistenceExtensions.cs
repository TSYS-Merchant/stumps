namespace Stumps
{

    using System;

    /// <summary>
    ///     A class that provides a set of extension methods used to preserve and retrieve the state of
    ///     <see cref="T:Stumps.Stump"/> objects.
    /// </summary>
    public static class StumpPersistenceExtensions
    {

        /// <summary>
        ///     Persists a specified <see cref="T:Stumps.Stump"/> to an archive file.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> to persist.</param>
        /// <param name="fileName">The name of the archive file.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="stump"/> is <c>null</c>.
        /// -or-
        /// <paramref name="fileName"/> is <c>null</c>.
        /// </exception>
        public static void SaveToArchive(this Stump stump, string fileName)
        {

            if (stump == null)
            {
                throw new ArgumentNullException("stump");
            }

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            throw new NotImplementedException();

        }

        /// <summary>
        ///     Persists a specified <see cref="T:Stumps.Stump"/> to a directory.
        /// </summary>
        /// <param name="stump">The <see cref="T:Stumps.Stump"/> to persist.</param>
        /// <param name="path">The path to the directory where the <see cref="T:Stumps.Stump"/> is saved.</param>
        public static void SaveToDirectory(this Stump stump, string path)
        {

            throw new NotImplementedException();

        }

    }

}
