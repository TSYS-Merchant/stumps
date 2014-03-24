namespace Stumps
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A custom dictionary that contains the headers in the context of an HTTP request or HTTP response.
    /// </summary>
    public class HttpHeaders : IHttpHeaders
    {

        private readonly Dictionary<string, string> _headers;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.HttpHeaders"/> class.
        /// </summary>
        public HttpHeaders()
        {
            _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
        ///     Gets a collection of the names of all the headers.
        /// </summary>
        /// <value>
        ///     The collection of names of all the headers.
        /// </value>
        public ICollection<string> HeaderNames
        {
            get { return _headers.Keys; }
        }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:Stumps.IHttpHeaders" /> is read-only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="T:Stumps.IHttpHeaders" /> is read-only; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///     Gets or sets the <see cref="System.String"/> value for the specified header name.
        /// </summary>
        /// <value>
        ///     The <see cref="System.String"/> value for the specified header name.
        /// </value>
        /// <param name="headerName">The name of the header.</param>
        public virtual string this[string headerName]
        {
            get
            {
                var keyValue = _headers.ContainsKey(headerName) ? _headers[headerName] : null;
                return keyValue;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(headerName) || value == null)
                {
                    // This should never happen in a real-world scenario, but fall out gracefully if it does
                    // rather than throwing an exception.
                    return;
                }

                if (_headers.ContainsKey(headerName))
                {
                    _headers[headerName] = value;
                }
                else
                {
                    _headers.Add(headerName, value);
                }

            }

        }

        /// <summary>
        ///     Clears all existing headers from the instance.
        /// </summary>
        public virtual void Clear()
        {
            _headers.Clear();
        }

        /// <summary>
        ///     Copies the elements of the <see cref="T:Stumps.IHttpHeaders"/> collection to another <see cref="T:Stumps.IHttpHeaders"/>.
        /// </summary>
        /// <param name="httpHeaders">The target <see cref="T:Stumps.IHttpHeaders"/>.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="httpHeaders"/> is <c>null</c>.</exception>
        public virtual void CopyTo(IHttpHeaders httpHeaders)
        {
                
            if (httpHeaders == null)
            {
                throw new ArgumentNullException("httpHeaders");
            }

            foreach (var headerName in this.HeaderNames)
            {
                httpHeaders[headerName] = this[headerName];
            }

        }

        /// <summary>
        ///     Removes the header with the specified name.
        /// </summary>
        /// <param name="headerName">The name of the header to remove.</param>
        /// <returns><c>true</c> if the header was found and removed; otherwise, <c>false</c>.</returns>
        public virtual bool Remove(string headerName)
        {
            var removed = _headers.Remove(headerName);
            return removed;
        }

    }

}
