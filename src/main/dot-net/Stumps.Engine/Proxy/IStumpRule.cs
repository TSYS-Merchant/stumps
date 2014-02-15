namespace Stumps.Proxy
{

    using Stumps.Http;

    public interface IStumpRule
    {

        bool IsMatch(IStumpsHttpRequest request);

    }

}