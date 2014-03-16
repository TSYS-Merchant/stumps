namespace Stumps.Http
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A custom dictionary that contains the headers in the context of an HTTP request or HTTP response.
    /// </summary>
    internal class HeaderDictionary : IHeaderDictionary
    {

        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.Http.HeaderDictionary"/> class.
        /// </summary>
        public HeaderDictionary()
        {
            _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified header name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="headerName">Name of the header.</param>
        /// <returns></returns>
        public string this[string headerName]
        {
            get
            {
                var keyValue = _headers.ContainsKey(headerName) ? _headers[headerName] : null;
                return keyValue;
            }
        }

        /// <summary>
        /// Gets the count of headers in the dictionary.
        /// </summary>
        /// <value>
        /// The count of headers in the dictionary.
        /// </value>
        public int Count
        {
            get { return _headers.Count; }
        }

        /// <summary>
        /// Gets a collection of the names of all the headers.
        /// </summary>
        /// <value>
        /// The collection of names of all the headers.
        /// </value>
        public ICollection<string> HeaderNames
        {
            get { return _headers.Keys; }
        }

        /// <summary>
        /// Adds or updates the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        /// <exception cref="System.ArgumentNullException">
        /// name
        /// or
        /// value
        /// </exception>
        public void AddOrUpdate(string name, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (_headers.ContainsKey(name))
            {
                _headers[name] = value;
            }
            else
            {
                _headers.Add(name, value);
            }

        }

        /// <summary>
        /// Clears all existing headers from the instance.
        /// </summary>
        public void Clear()
        {
            _headers.Clear();
        }

        /// <summary>
        /// Removes the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header to remove.</param>
        /// <returns>
        ///   <c>true</c> if the header was found and removed; otherwise, <c>false</c>.
        /// </returns>
        public bool Remove(string name)
        {
            var removed = _headers.Remove(name);
            return removed;
        }

    }

}
