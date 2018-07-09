namespace Stumps
{
    using System;

    /// <summary>
    ///     An interface that represents the most basic components of an HTTP request and an HTTP response.
    /// </summary>
    public interface IStumpsHttpContextPart
    {
        /// <summary>
        ///     Gets the length of the HTTP body.
        /// </summary>
        /// <value>
        ///     The length of the HTTP body.
        /// </value>
        int BodyLength { get; }

        /// <summary>
        ///     Gets the collection of HTTP headers.
        /// </summary>
        /// <value>
        ///     The collection of HTTP headers.
        /// </value>
        IHttpHeaders Headers { get; }

        /// <summary>
        ///     Gets the bytes for the HTTP body.
        /// </summary>
        /// <returns>
        ///     An array of <see cref="Byte"/> values representing the HTTP body.
        /// </returns>
        byte[] GetBody();
    }
}
