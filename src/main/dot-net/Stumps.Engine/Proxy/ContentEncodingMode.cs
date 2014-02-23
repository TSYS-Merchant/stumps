namespace Stumps.Proxy
{

    /// <summary>
    ///     Provides a list of possible content encoding modes.
    /// </summary>
    public enum ContentEncodingMode
    {

        /// <summary>
        ///     The content is to be encoded using a specified encoding methodd.
        /// </summary>
        Encode = 0,

        /// <summary>
        ///     The content is to be decoded using a specified encoding method.
        /// </summary>
        Decode = 1

    }

}