﻿namespace Stumps.Proxy {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using Stumps.Utility;

    public class ContentEncoding {

        private static Dictionary<string, Func<Stream, ContentEncodingMode, Stream>> _streamEncoders =
            new Dictionary<string, Func<Stream, ContentEncodingMode, Stream>>(StringComparer.OrdinalIgnoreCase) {
                { "gzip", createGzipStream },
                { "deflate", createDeflateStream }
            };

        public ContentEncoding(string encodingMethod) {

            encodingMethod = encodingMethod ?? string.Empty;
            encodingMethod = encodingMethod.Trim();

            this.Method = encodingMethod;

        }

        public string Method {
            get;
            private set;
        }

        public byte[] Encode(byte[] value) {

            if ( !_streamEncoders.ContainsKey(this.Method) || value == null ) {
                return value;
            }

            byte[] output = null;

            using ( var inputStream = new MemoryStream(value) ) {

                using ( var outputStream = new MemoryStream() ) {

                    using ( var encoderStream = _streamEncoders[this.Method](outputStream, ContentEncodingMode.Encode) ) {

                        inputStream.CopyTo(encoderStream);
                        encoderStream.Flush();

                    }

                    output = outputStream.ToArray();

                }

            }

            return output;

        }

        public byte[] Decode(byte[] value) {

            if ( !_streamEncoders.ContainsKey(this.Method) || value == null ) {
                return value;
            }

            byte[] output = null;

            using ( var inputStream = new MemoryStream(value) ) {

                using ( var outputStream = new MemoryStream() ) {

                    using ( var encoderStream = _streamEncoders[this.Method](inputStream, ContentEncodingMode.Decode) ) {

                        encoderStream.CopyTo(outputStream);
                        output = outputStream.ToArray();

                    }

                }

            }

            return output;

        }

        private static Stream createGzipStream(Stream stream, ContentEncodingMode mode) {

            var compressionMode = (mode == ContentEncodingMode.Encode ? CompressionMode.Compress : CompressionMode.Decompress);
            var compressionStream = new GZipStream(stream, compressionMode, true);
            return compressionStream;

        }

        private static Stream createDeflateStream(Stream stream, ContentEncodingMode mode) {

            var compressionMode = (mode == ContentEncodingMode.Encode ? CompressionMode.Compress : CompressionMode.Decompress);
            var compressionStream = new DeflateStream(stream, compressionMode, true);
            return compressionStream;

        }

    }

}