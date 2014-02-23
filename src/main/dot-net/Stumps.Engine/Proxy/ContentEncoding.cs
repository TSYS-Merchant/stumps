namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;

    /// <summary>
    ///     A class that encodes and decodes the body of an HTTP request or an HTTP response for a specified HTTP content encoding method.
    /// </summary>
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

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.ContentEncoding"/> class.
        /// </summary>
        /// <param name="encodingMethod">The HTTP encoding method.</param>
        public ContentEncoding(string encodingMethod)
        {

            encodingMethod = encodingMethod ?? string.Empty;
            encodingMethod = encodingMethod.Trim();

            this.Method = encodingMethod;

        }

        /// <summary>
        /// Gets the HTTP encoding method.
        /// </summary>
        /// <value>
        /// The HTTP encoding method.
        /// </value>
        public string Method { get; private set; }

        /// <summary>
        ///     Decodes the specified value.
        /// </summary>
        /// <param name="value">The byte array to decode.</param>
        /// <returns>
        ///     The decoded form of the <paramref name="value"/>.
        /// </returns>
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

        /// <summary>
        ///     Encodes the specified value.
        /// </summary>
        /// <param name="value">The byte array to encode.</param>
        /// <returns>
        ///     The encoded form of the <paramref name="value"/>.
        /// </returns>
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

        /// <summary>
        ///     Creates a new instance of a deflate stream initialized to the specified mode.
        /// </summary>
        /// <param name="stream">The stream used to initialize the deflate stream.</param>
        /// <param name="mode">The content encoding method.</param>
        /// <returns>
        ///     A <see cref="T:System.IO.Stream"/> representing the created stream.
        /// </returns>
        private static Stream CreateDeflateStream(Stream stream, ContentEncodingMode mode)
        {

            var compressionMode = mode == ContentEncodingMode.Encode
                                       ? CompressionMode.Compress
                                       : CompressionMode.Decompress;
            var compressionStream = new DeflateStream(stream, compressionMode, true);

            return compressionStream;

        }

        /// <summary>
        ///     Creates a new instance of a Gzip stream initialized to the specified mode.
        /// </summary>
        /// <param name="stream">The stream used to initialize the Gzip stream.</param>
        /// <param name="mode">The content encoding mode.</param>
        /// <returns>
        ///     A <see cref="T:System.IO.Stream"/> representing the created stream.
        /// </returns>
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