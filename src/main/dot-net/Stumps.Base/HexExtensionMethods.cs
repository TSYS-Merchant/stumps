namespace Stumps
{

    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    ///     A class that provides a conversion between byte arrays an a HEX representation.
    /// </summary>
    public static class HexExtensionMethods
    {

        /// <summary>
        ///     Converts a byte array into a hex-encoded string.
        /// </summary>
        /// <param name="byteArray">The byte array to encode.</param>
        /// <returns>A hex-encoded representation of <paramref name="byteArray"/>.</returns>
        public static string ToHexString(this byte[] byteArray)
        {

            if (byteArray == null)
            {
                return null;
            }

            var result = BitConverter.ToString(byteArray);
            result = result.Replace("-", string.Empty);
            return result;

        }

        /// <summary>
        ///     Converts a hex-encoded string into a byte array.
        /// </summary>
        /// <param name="hexString">The hex-encoded string.</param>
        /// <returns>An array of <see cref="T:System.Byte"/> values decoded from <paramref name="hexString"/>.</returns>
        public static byte[] ToByteArray(this string hexString)
        {

            if (hexString == null)
            {
                return null;
            }

            var bytes = new List<byte>();

            for (var i = 0; i < hexString.Length; i += 2)
            {
                var s = hexString.Substring(i, 2);
                var b = byte.Parse(s, NumberStyles.HexNumber);
                bytes.Add(b);
            }

            return bytes.ToArray();

        }

    }

}
