namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    public class ContentEncoding
    {

        private static readonly Dictionary<string, Func<Stream, ContentEncodingMode, Stream>> StreamEncoders =
            new Dictionary<string, Func<Stream, ContentEncodingMode, Stream>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "gzip", CreateGzipStream
                },
                {
                    "deflate", CreateDeflateStream
                }
            };

        public ContentEncoding(string encodingMethod)
        {

            encodingMethod = encodingMethod ?? string.Empty;
            encodingMethod = encodingMethod.Trim();

            this.Method = encodingMethod;

        }

        public string Method { get; private set; }

        public byte[] Encode(byte[] value)
        {

            if (!StreamEncoders.ContainsKey(this.Method) || value == null)
            {
                return value;
            }

            byte[] output;

            using (var inputStream = new MemoryStream(value))
            {

                using (var outputStream = new MemoryStream())
                {

                    using (var encoderStream = StreamEncoders[this.Method](outputStream, ContentEncodingMode.Encode))
                    {

                        inputStream.CopyTo(encoderStream);
                        encoderStream.Flush();

                    }

                    output = outputStream.ToArray();

                }

            }

            return output;

        }

        public byte[] Decode(byte[] value)
        {

            if (!StreamEncoders.ContainsKey(this.Method) || value == null)
            {
                return value;
            }

            byte[] output;

            using (var inputStream = new MemoryStream(value))
            {

                using (var outputStream = new MemoryStream())
                {

                    using (var encoderStream = StreamEncoders[this.Method](inputStream, ContentEncodingMode.Decode))
                    {

                        encoderStream.CopyTo(outputStream);
                        output = outputStream.ToArray();

                    }

                }

            }

            return output;

        }

        private static Stream CreateDeflateStream(Stream stream, ContentEncodingMode mode)
        {

            var compressionMode = mode == ContentEncodingMode.Encode
                                       ? CompressionMode.Compress
                                       : CompressionMode.Decompress;
            var compressionStream = new DeflateStream(stream, compressionMode, true);

            return compressionStream;

        }

        private static Stream CreateGzipStream(Stream stream, ContentEncodingMode mode)
        {

            var compressionMode = mode == ContentEncodingMode.Encode 
                                       ? CompressionMode.Compress
                                       : CompressionMode.Decompress;
            var compressionStream = new GZipStream(stream, compressionMode, true);
            return compressionStream;

        }

    }

}