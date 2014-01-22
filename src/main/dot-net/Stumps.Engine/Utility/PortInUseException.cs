using System;

public class PortInUseException : Exception
{
    public PortInUseException()
    {
    }

    public PortInUseException(string message)
        : base(message)
    {
    }

    public PortInUseException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
