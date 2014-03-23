namespace Stumps.Web
{

    using System.Text;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides helpers for Nancy modules.
    /// </summary>
    internal static class NancyModuleHelper
    {

        /// <summary>
        ///     Generates the body text from a recorded context part.
        /// </summary>
        /// <param name="part">The <see cref="T:Stumps.Server.IRecordedContextPart"/>.</param>
        /// <returns>A <see cref="T:System.String"/> containing the text of the body.</returns>
        public static string GenerateBodyText(IRecordedContextPart part)
        {

            var result = Encoding.UTF8.GetString(part.Body);
            return result;

        }

    }

}