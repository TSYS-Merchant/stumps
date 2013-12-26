namespace Stumps {

    using System;

    interface IStumpModule : IDisposable {

        void Start();

        void Shutdown();

    }

}
