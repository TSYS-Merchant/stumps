namespace Stumps
{

    using System.Collections.Generic;

    public interface IHeaderDictionary
    {

        string this[string key] { get; }

        int Count { get; }

        ICollection<string> HeaderNames { get; }

        string Add(string name, string value);

        void Clear();

        string Remove(string name, string value);

    }

}
