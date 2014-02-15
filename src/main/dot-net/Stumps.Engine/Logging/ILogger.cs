namespace Stumps.Logging
{

    using System;

    public interface ILogger
    {

        void LogInfo(string data);

        void LogException(string location, Exception exception);

    }

}