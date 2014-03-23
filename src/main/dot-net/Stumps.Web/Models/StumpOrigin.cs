namespace Stumps.Web.Models
{

    /// <summary>
    ///     An enumeration that describes the origin of a Stump.
    /// </summary>
    public enum StumpOrigin
    {

        /// <summary>
        /// The Stump was not created from any originating source.
        /// </summary>
        Nothing = 0,

        /// <summary>
        ///     The Stump was created from a recorded HTTP request and response.
        /// </summary>
        RecordedContext = 1,

        /// <summary>
        ///     The Stump was created from an existing Stump.
        /// </summary>
        ExistingStump = 2

    }

}