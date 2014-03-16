namespace Stumps
{

    using System.Collections.Generic;

    public interface IHeaderDictionary
    {

        /// <summary>
        /// Gets the <see cref="System.String"/> value for the specified header name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/> value for the specified header name.
        /// </value>
        /// <param name="headerName">The name of the header.</param>
        string this[string headerName] { get; }

        /// <summary>
        ///     Gets the count of headers in the dictionary.
        /// </summary>
        /// <value>
        /// The count of headers in the dictionary.
        /// </value>
        int Count { get; }

        /// <summary>
        /// Gets a collection of the names of all the headers.
        /// </summary>
        /// <value>
        /// The collection of names of all the headers.
        /// </value>
        ICollection<string> HeaderNames { get; }

        /// <summary>
        ///     Adds or updates the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        void AddOrUpdate(string name, string value);

        /// <summary>
        ///     Clears all existing headers from the instance.
        /// </summary>
        void Clear();

        /// <summary>
        ///     Removes the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header to remove.</param>
        /// <returns><c>true</c> if the header was found and removed; otherwise, <c>false</c>.</returns>
        bool Remove(string name);

    }

}
