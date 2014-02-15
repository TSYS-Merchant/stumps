namespace Stumps.Http
{

    internal interface IHttpHandler
    {

        ProcessHandlerResult ProcessRequest(IStumpsHttpContext context);

    }

}