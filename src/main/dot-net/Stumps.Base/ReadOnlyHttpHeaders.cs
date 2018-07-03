namespace Stumps
{
    using System;

    /// <summary>
    ///     A read-only implementation of <see cref="T:Stumps.IHttpHeaders" /> that can be used to protect HTTP requests from being altered.
    /// </summary>
    internal class ReadOnlyHttpHeaders : HttpHeaders
    {
        /// <summary>
        ///     Gets a value indicating whether the <see cref="T:Stumps.IHttpHeaders" /> is read-only.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="T:Stumps.IHttpHeaders" /> is read-only; otherwise, <c>false</c>.
        /// </value>
        public override bool IsReadOnly
        {
            get => true;
        }
        
        /// <summary>
        ///     Gets or sets the <see cref="System.String"/> value for the specified header name.
        /// </summary>
        /// <value>
        ///     The <see cref="System.String"/> value for the specified header name.
        /// </value>
        /// <param name="headerName">The name of the header.</param>
        /// <exception cref="System.NotSupportedException">Thrown when altering the value of a header.</exception>
        public override string this[string headerName]
        {
            get => base[headerName];
            set => throw new NotSupportedException();
        }

        /// <summary>
        ///     Clears all existing headers from the instance.
        /// </summary>
        /// <exception cref="System.NotSupportedException">Always thrown.</exception>
        public override void Clear() => throw new NotSupportedException();

        /// <summary>
        ///     Removes the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header to remove.</param>
        /// <returns>
        ///   <c>true</c> if the header was found and removed; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Always thrown.</exception>
        public override bool Remove(string name) => throw new NotSupportedException();

        /// <summary>
        ///     Clears all existing headers from the instance.
        /// </summary>
        internal void ClearInternal() => base.Clear();

        /// <summary>
        ///     Adds or updates an existing header.
        /// </summary>
        /// <param name="name">The name of the header to add or update.</param>
        /// <param name="value">The value of the header.</param>
        internal void AddOrUpdateInternal(string name, string value) => base[name] = value;

        /// <summary>
        ///     Removes the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the header to remove.</param>
        /// <returns>
        ///   <c>true</c> if the header was found and removed; otherwise, <c>false</c>.
        /// </returns>
        internal bool RemoveInternal(string name) => base.Remove(name);
    }
}
