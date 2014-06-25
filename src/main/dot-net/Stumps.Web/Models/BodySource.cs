namespace Stumps.Web.Models
{

    /// <summary>
    ///     An enumeration that represents the origin of the HTTP body.
    /// </summary>
    public enum BodySource
    {

        /// <summary>
        ///     There is no HTTP body.
        /// </summary>
        EmptyBody = 0,

        /// <summary>
        ///     The HTTP body is the same as the originating source.
        /// </summary>
        Origin = 1,

        /// <summary>
        ///     The HTTP body is a custom modified value.
        /// </summary>
        Modified = 2

    }

}