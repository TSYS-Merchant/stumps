namespace Stumps
{

    /// <summary>
    ///     A class that represents a set of String based functions.
    /// </summary>
    public static class TextAnalyzer
    {

        /// <summary>
        /// The allowed threshold for a byte array to be considered text.
        /// </summary>
        public const double TextThreshold = 0.05;

        /// <summary>
        /// Determines whether the specified buffer is text.
        /// </summary>
        /// <param name="buffer">The buffer to determine is text.</param>
        /// <returns>
        ///     <c>true</c> if the buffer represents text; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsText(byte[] buffer)
        {

            if (buffer == null || buffer.Length == 0)
            {
                return false;
            }

            var isText = true;
            var badCount = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] > 126)
                {
                    badCount++;
                }
            }

            var badPercent = (float)badCount / buffer.Length;

            if (badPercent > TextAnalyzer.TextThreshold)
            {
                isText = false;
            }

            return isText;

        }

    }

}