namespace Stumps.Http
{
    /// <summary>
    ///     Provides the result of a class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> 
    ///     interface when processing an incoming request.
    /// </summary>
    internal enum ProcessHandlerResult
    {
        /// <summary>
        ///     The HTTP request can continue to be processed.
        /// </summary>
        Continue = 0,

        /// <summary>
        ///     The HTTP request should not be processed any more.
        /// </summary>
        Terminate = 1,

        /// <summary>
        ///     The HTTP request should not be processed any more and the connection should be immediately terminated.
        /// </summary>
        DropConnection = 2
    }
}