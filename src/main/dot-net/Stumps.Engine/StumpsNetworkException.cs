namespace Stumps
{

    using System;
    using System.Runtime.Serialization;

    [Serializable]
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

        protected StumpsNetworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

}