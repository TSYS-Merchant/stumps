namespace Stumps.Proxy
{

    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents the basic information for a part of an HTTP context.
    /// </summary>
    internal interface IRecordedContextPart
    {

        /// <summary>
        ///     Gets or sets the byte array representing the body of the context part.
        /// </summary>
        /// <value>
        ///     The byte array representing the body of the context part.
        /// </value>
        byte[] Body { get; set; }

        /// <summary>
        ///     Gets or sets the content type of the body.
        /// </summary>
        /// <value>
        ///     The content type of the body.
        /// </value>
        string BodyContentType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body"/> of the current instance is an image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body"/> of the current instance is an image; otherwise, <c>false</c>.
        /// </value>
        bool BodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body"/> of the current instance is text.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body"/> of the current instance is text; otherwise, <c>false</c>.
        /// </value>
        bool BodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets the headers in the context part.
        /// </summary>
        /// <value>
        ///     The headers in the context part.
        /// </value>
        IList<HttpHeader> Headers { get; set; }

        /// <summary>
        ///     Finds the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the specified header.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Proxy.HttpHeader"/> with the specified <paramref name="name"/>.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if an <see cref="T:Stumps.Proxy.HttpHeader"/> is not found.
        /// </remarks>
        HttpHeader FindHeader(string name);

    }

}