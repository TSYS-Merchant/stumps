namespace Stumps
{

    using System;

    public class StumpsNetworkException : Exception
    {

        public StumpsNetworkException()
        {
        }

        public StumpsNetworkException(string message) : base(message)
        {
        }

        public StumpsNetworkException(string message, Exception inner) : base(message, inner)
        {
        }

    }

}