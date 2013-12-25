namespace Stumps.Utility {

    using System;
    using System.IO;
    using System.IO.Compression;

    internal static class CompressionUtility {

        public static byte[] CompressDeflateByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            MemoryStream outputStream = null;
            byte[] responseBytes = null;

            try {

                outputStream = new MemoryStream();

                var compressionStream = new DeflateStream(outputStream, CompressionMode.Compress);
                compressionStream.Write(value, 0, value.Length);
                compressionStream.Flush();
                
                responseBytes = outputStream.ToArray();

                compressionStream.Close();

            }
            catch {
                outputStream.Close();
                throw;
            }

            return responseBytes;

        }

        public static byte[] CompressGzipByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            MemoryStream outputStream = null;
            byte[] responseBytes = null;

            try {

                outputStream = new MemoryStream();

                var compressionStream = new GZipStream(outputStream, CompressionMode.Compress);
                compressionStream.Write(value, 0, value.Length);
                compressionStream.Flush();

                responseBytes = outputStream.ToArray();

                compressionStream.Close();

            }
            catch {
                outputStream.Close();
                throw;
            }

            return responseBytes;

        }

        public static byte[] DecompressDeflateByteArray(byte[] value) {


            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            MemoryStream inputStream = null;
            MemoryStream outputStream = null;
            byte[] responseBytes = null;

            try {

                inputStream = new MemoryStream(value);
                outputStream = new MemoryStream();

                var compressionStream = new DeflateStream(inputStream, CompressionMode.Decompress);

                var buffer = new byte[StreamUtility.BufferSize];
                var bytesRead = 0;

                while ( ( bytesRead = compressionStream.Read(buffer, 0, StreamUtility.BufferSize) ) > 0 ) {
                    outputStream.Write(buffer, 0, bytesRead);
                }

                responseBytes = outputStream.ToArray();

                compressionStream.Close();

            }
            catch {
                inputStream.Close();
                throw;
            }
            finally {
                outputStream.Close();
            }

            return responseBytes;

        }

        public static byte[] DecompressGzipByteArray(byte[] value) {

            if ( value == null ) {
                throw new ArgumentNullException("value");
            }

            MemoryStream inputStream = null;
            MemoryStream outputStream = null;
            byte[] responseBytes = null;

            try {

                inputStream = new MemoryStream(value);
                outputStream = new MemoryStream();

                var compressionStream = new GZipStream(inputStream, CompressionMode.Decompress);

                var buffer = new byte[StreamUtility.BufferSize];
                var bytesRead = 0;

                while ( ( bytesRead = compressionStream.Read(buffer, 0, StreamUtility.BufferSize) ) > 0 ) {
                    outputStream.Write(buffer, 0, bytesRead);
                }

                responseBytes = outputStream.ToArray();

                compressionStream.Close();

            }
            catch {
                inputStream.Close();
                throw;
            }
            finally {
                outputStream.Close();
            }

            return responseBytes;

        }

    }

}
