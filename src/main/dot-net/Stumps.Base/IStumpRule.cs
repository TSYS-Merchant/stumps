namespace Stumps
{

    /// <summary>
    ///     An interface that represents a Stumps rule when attempting to match against an <see cref="T:Stumps.IStumpsHttpRequest"/>.
    /// </summary>
    public interface IStumpRule
    {

        /// <summary>
        ///     Determines whether the specified request matches the rule.
        /// </summary>
        /// <param name="request">The <see cref="T:Stumps.IStumpsHttpRequest"/> to evaluate.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="request"/> matches the rule, otherwise, <c>false</c>.
        /// </returns>
        bool IsMatch(IStumpsHttpRequest request);

    }

}