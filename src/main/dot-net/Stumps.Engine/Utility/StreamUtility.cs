namespace Stumps.Utility
{

    using System;
    using System.IO;
    using System.Text;

    internal static class StreamUtility
    {

        public const int BufferSize = 4096;

        public static byte[] ConvertStreamToByteArray(Stream stream)
        {

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var buffer = new byte[StreamUtility.BufferSize];
            byte[] outArray;

            stream.Position = 0;

            using (var ms = new MemoryStream())
            {

                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, StreamUtility.BufferSize)) > 0)
                {
                    ms.Write(buffer, 0, bytesRead);
                }

                outArray = ms.ToArray();

            }

            stream.Position = 0;

            return outArray;

        }

        public static void CopyStream(Stream inputStream, Stream outputStream)
        {

            StreamUtility.CopyStream(inputStream, outputStream, -1);

        }

        public static void CopyStream(Stream inputStream, Stream outputStream, int startingPosition)
        {

            if (inputStream == null)
            {
                throw new ArgumentNullException("inputStream");
            }

            if (outputStream == null)
            {
                throw new ArgumentNullException("outputStream");
            }

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

        public static void WriteUtf8StringToStream(string value, Stream stream)
        {

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            WriteStringToStream(value, stream, Encoding.UTF8);

        }

        private static void WriteStringToStream(string value, Stream stream, Encoding encoding)
        {

            using (var writer = new StreamWriter(stream, encoding))
            {
                writer.Write(value);
            }

        }

    }

}