namespace Stumps.Server.Utility
{

    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    ///     A class that creates random identifier used by Stumps.
    /// </summary>
    internal static class RandomGenerator
    {

        public const int KeySize = 7;

        private static readonly char[] RandomCharacters = new char[]
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
        };

        /// <summary>
        /// Generates a new unique identifier.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.String"/> containing a identifier.
        /// </returns>
        public static string GenerateIdentifier()
        {

            string identifier;

            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {

                var data = new byte[RandomGenerator.KeySize];
                cryptoProvider.GetNonZeroBytes(data);

                var sb = new StringBuilder();
                for (int i = 0; i < RandomGenerator.KeySize; i++)
                {
                    sb.Append(RandomGenerator.RandomCharacters[data[i] % 36]);
                }

                identifier = sb.ToString();

            }

            return identifier;

        }

    }

}