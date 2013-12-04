namespace Stumps.Utility {

    using System.IO;

    internal static class StringUtility {

        public static bool IsText(Stream stream) {

            if ( stream.Length == 0 ) {
                return false;
            }

            var buffer = StreamUtility.ConvertStreamToByteArray(stream);

            var isArrayText = StringUtility.IsText(buffer);

            return isArrayText;

        }

        public static bool IsText(byte[] buffer) {

            if ( buffer == null || buffer.Length == 0 ) {
                return false;
            }

            var isText = true;
            var badCount = 0;

            for ( int i = 0; i < buffer.Length; i++ ) {
                if ( buffer[i] > 126 ) {
                    badCount++;
                }
            }

            var badPercent = (float) badCount / buffer.Length;
            if ( badPercent > 0.05 ) {
                isText = false;
            }

            return isText;

        }

    }

}
