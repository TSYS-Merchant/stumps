namespace Stumps.Utility {

    using System;
    using System.IO;
    using System.IO.Compression;

    internal static class CompressionUtility {

        public static byte[] CompressDeflateByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            byte[] outBytes;

            using ( var outputStream = new MemoryStream() ) {

                using ( var compressionStream = new DeflateStream(outputStream, CompressionMode.Compress) ) {

                    compressionStream.Write(value, 0, value.Length);

                }

                outBytes = outputStream.ToArray();

            }

            return outBytes;

        }

        public static byte[] CompressGzipByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            byte[] outBytes;

            using ( var outputStream = new MemoryStream() ) {

                using ( var compressionStream = new GZipStream(outputStream, CompressionMode.Compress) ) {

                    compressionStream.Write(value, 0, value.Length);

                }

                outBytes = outputStream.ToArray();

            }

            return outBytes;

        }


        public static byte[] DecompressDeflateByteArray(byte[] value) {


            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            byte[] outBytes;

            using ( var inputStream = new MemoryStream(value) ) {

                using ( var compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress) ) {

                    using ( var outputStream = new MemoryStream() ) {

                        var buffer = new byte[StreamUtility.BufferSize];
                        var bytesRead = 0;

                        while ( (bytesRead = compressionStream.Read(buffer, 0, StreamUtility.BufferSize)) > 0 ) {
                            outputStream.Write(buffer, 0, bytesRead);
                        }

                        outBytes = outputStream.ToArray();

                    }
                }
            }

            return outBytes;

        }

        public static byte[] DecompressGzipByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            byte[] outBytes;

            using ( var inputStream = new MemoryStream(value) ) {

                using ( var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress) ) {

                    using ( var outputStream = new MemoryStream() ) {

                        var buffer = new byte[StreamUtility.BufferSize];
                        var bytesRead = 0;

                        while ( (bytesRead = compressionStream.Read(buffer, 0, StreamUtility.BufferSize)) > 0 ) {
                            outputStream.Write(buffer, 0, bytesRead);
                        }

                        outBytes = outputStream.ToArray();

                    }
                }
            }

            return outBytes;

        }

    }

}
