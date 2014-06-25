namespace Stumps.Server
{

    /// <summary>
    ///     An enumeration that represents decoding is required to examing the HTTP body.
    /// </summary>
    public enum ContentDecoderHandling
    {

        /// <summary>
        ///     The decode not required because it has already been performed.
        /// </summary>
        DecodeNotRequired = 0,

        /// <summary>
        ///     The decode is required because it has not been performed.
        /// </summary>
        DecodeRequired = 1

    }

}
