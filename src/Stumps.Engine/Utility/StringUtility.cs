namespace Stumps.Utility {

    using System.IO;

    internal static class StringUtility {

        public const double TextThreshold = 0.05;

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
            
            if ( badPercent > StringUtility.TextThreshold ) {
                isText = false;
            }

            return isText;

        }

    }

}
