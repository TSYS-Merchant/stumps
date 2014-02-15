namespace Stumps.Web
{

    using System.Text;
    using Stumps.Proxy;

    internal static class NancyModuleHelper
    {

        public static string GenerateBodyText(IRecordedContextPart part)
        {

            var result = Encoding.UTF8.GetString(part.Body);
            return result;

        }

    }

}