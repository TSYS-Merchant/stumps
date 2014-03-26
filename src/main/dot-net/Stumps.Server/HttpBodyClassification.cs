namespace Stumps.Server
{

    /// <summary>
    ///     Provides a classification of the HTTP body after being analyzed.
    /// </summary>
    public enum HttpBodyClassification
    {

        /// <summary>
        ///     The HTTP body is empty.
        /// </summary>
        Empty = 0,

        /// <summary>
        ///     The HTTP body contains binary information.
        /// </summary>
        Binary = 1,

        /// <summary>
        ///     The HTTP body contains an image.
        /// </summary>
        Image = 2,

        /// <summary>
        ///     The HTTP body contains text.
        /// </summary>
        Text = 3

    }

}
