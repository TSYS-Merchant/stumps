namespace Stumps
{

    using System;

    internal interface IStumpModule : IDisposable
    {

        void Start();

        void Shutdown();

    }

}