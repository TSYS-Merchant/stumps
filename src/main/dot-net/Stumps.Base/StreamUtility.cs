namespace Stumps
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    ///     A class that represents a set of Stream based functions.
    /// </summary>
    public static class StreamUtility
    {
        private const int BufferSize = 4096;

        /// <summary>
        ///     Converts a stream to a byte array.
        /// </summary>
        /// <param name="stream">The stream to convert to a byte array.</param>
        /// <returns>
        ///     An array of bytes contained within the specified <paramref name="stream"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        public static async Task<byte[]> ConvertStreamToByteArray(Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            byte[] streamAsBytes;

            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                streamAsBytes = ms.ToArray();
            }

            return streamAsBytes;
        }

        /// <summary>
        /// Copies data from an input stream to an output stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        public static async Task CopyStream(Stream inputStream, Stream outputStream)
        {
            await StreamUtility.CopyStream(inputStream, outputStream, -1);
        }

        /// <summary>
        /// Copies data from an input stream to an output stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="startingPosition">The starting position of the input stream.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputStream"/> is <c>null</c>.
        /// or
        /// <paramref name="outputStream"/> is <c>null</c>.
        /// </exception>
        public static async Task CopyStream(Stream inputStream, Stream outputStream, int startingPosition)
        {
            inputStream = inputStream ?? throw new ArgumentNullException(nameof(inputStream));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            var buffer = new byte[StreamUtility.BufferSize];
            int bytesRead;

            if (startingPosition > -1)
            {
                inputStream.Position = startingPosition;
            }

            while ((bytesRead = await inputStream.ReadAsync(buffer, 0, StreamUtility.BufferSize)) > 0)
            {
                await outputStream.WriteAsync(buffer, 0, bytesRead);
            }

            if (startingPosition > -1)
            {
                inputStream.Position = startingPosition;
            }
        }

        /// <summary>
        ///     Writes the UTF8 string to a stream.
        /// </summary>
        /// <param name="value">The value to write to the stream.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// or
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public static async Task WriteUtf8StringToStream(string value, Stream stream)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            await WriteStringToStream(value, stream, Encoding.UTF8);
        }

        /// <summary>
        ///     Writes the string to a stream.
        /// </summary>
        /// <param name="value">The value to write to the stream.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The encoding used when writing to the stream.</param>
        private static async Task WriteStringToStream(string value, Stream stream, Encoding encoding)
        {
            using (var writer = new StreamWriter(stream, encoding))
            {
                await writer.WriteAsync(value);
            }
        }
    }
}