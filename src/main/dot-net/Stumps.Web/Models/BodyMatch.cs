namespace Stumps.Web.Models
{
    /// <summary>
    ///     An enumeration that describes the type of matching used against the body of an HTTP request.
    /// </summary>
    public enum BodyMatch
    {
        /// <summary>
        ///     The HTTP body can be anything.
        /// </summary>
        IsAnything = 0,

        /// <summary>
        ///     The HTTP body must be blank.
        /// </summary>
        IsBlank = 1,

        /// <summary>
        ///     The HTTP body is anything but cannot be blank.
        /// </summary>
        IsNotBlank = 2,

        /// <summary>
        ///     The HTTP body contains specified text.
        /// </summary>
        ContainsText = 3,

        /// <summary>
        ///     The HTTP body must match a known value.
        /// </summary>
        ExactMatch = 4
    }
}