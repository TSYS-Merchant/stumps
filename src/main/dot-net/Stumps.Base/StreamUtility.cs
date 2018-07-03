namespace Stumps
{
    using System;
    using System.IO;
    using System.Text;

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
        /// <exception cref="System.ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        public static byte[] ConvertStreamToByteArray(Stream stream)
        {
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            byte[] streamAsBytes;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                streamAsBytes = ms.ToArray();
            }

            return streamAsBytes;
        }

        /// <summary>
        /// Copies data from an input stream to an output stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        public static void CopyStream(Stream inputStream, Stream outputStream)
        {
            StreamUtility.CopyStream(inputStream, outputStream, -1);
        }

        /// <summary>
        /// Copies data from an input stream to an output stream.
        /// </summary>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="startingPosition">The starting position of the input stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="inputStream"/> is <c>null</c>.
        /// or
        /// <paramref name="outputStream"/> is <c>null</c>.
        /// </exception>
        public static void CopyStream(Stream inputStream, Stream outputStream, int startingPosition)
        {
            inputStream = inputStream ?? throw new ArgumentNullException(nameof(inputStream));
            outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));

            var buffer = new byte[StreamUtility.BufferSize];
            int bytesRead;

            if (startingPosition > -1)
            {
                inputStream.Position = startingPosition;
            }

            while ((bytesRead = inputStream.Read(buffer, 0, StreamUtility.BufferSize)) > 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is <c>null</c>.
        /// or
        /// <paramref name="stream"/> is <c>null</c>.
        /// </exception>
        public static void WriteUtf8StringToStream(string value, Stream stream)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));
            stream = stream ?? throw new ArgumentNullException(nameof(stream));

            WriteStringToStream(value, stream, Encoding.UTF8);
        }

        /// <summary>
        ///     Writes the string to a stream.
        /// </summary>
        /// <param name="value">The value to write to the stream.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding">The encoding used when writing to the stream.</param>
        private static void WriteStringToStream(string value, Stream stream, Encoding encoding)
        {
            using (var writer = new StreamWriter(stream, encoding))
            {
                writer.Write(value);
            }
        }
    }
}