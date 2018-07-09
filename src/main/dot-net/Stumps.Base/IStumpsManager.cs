namespace Stumps
{
    using System;

    /// <summary>
    ///     An interface for a class that manages a collection of <see cref="Stump"/> objects.
    /// </summary>
    internal interface IStumpsManager : IDisposable
    {
        /// <summary>
        /// Gets the count of Stumps in the collection.
        /// </summary>
        /// <value>
        /// The count of Stumps in the collection.
        /// </value>
        int Count { get; }

        /// <summary>
        ///     Adds a new <see cref="Stump" /> to the collection.
        /// </summary>
        /// <param name="stump">The <see cref="Stump" /> to add to the collection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stump"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">A <see cref="Stump" /> with the same identifier already exists.</exception>
        void AddStump(Stump stump);

        /// <summary>
        ///     Deletes all stumps from the collection.
        /// </summary>
        void DeleteAll();

        /// <summary>
        ///     Deletes the specified stump from the collection.
        /// </summary>
        /// <param name="stumpId">The  unique identifier for the stump to remove.</param>
        void DeleteStump(string stumpId);

        /// <summary>
        ///     Finds an existing stump.
        /// </summary>
        /// <param name="stumpId">The unique identifier for the Stump.</param>
        /// <returns>
        ///     A <see cref="Stump"/> with the specified <paramref name="stumpId"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stump is not found.
        /// </remarks>
        Stump FindStump(string stumpId);

        /// <summary>
        ///     Finds the Stump that matches an incoming HTTP request.
        /// </summary>
        /// <param name="context">The incoming HTTP request context.</param>
        /// <returns>
        ///     A <see cref="Stump"/> that matches the incoming HTTP request.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a matching Stump is not found.
        /// </remarks>
        Stump FindStumpForContext(IStumpsHttpContext context);
    }
}
