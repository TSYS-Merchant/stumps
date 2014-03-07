namespace Stumps
{

    using System;

    /// <summary>
    ///     An interface that represents the complete context for an HTTP request.
    /// </summary>
    public interface IStumpsHttpContext : IDisposable
    {

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpRequest"/> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpRequest"/> object for the current HTTP request.
        /// </value>
        IStumpsHttpRequest Request { get; }

        /// <summary>
        ///     Gets the <see cref="T:Stumps.IStumpsHttpResponse"/> object for the current HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="T:Stumps.IStumpsHttpResponse"/> object for the current HTTP request.
        /// </value>
        IStumpsHttpResponse Response { get; }

    }

}