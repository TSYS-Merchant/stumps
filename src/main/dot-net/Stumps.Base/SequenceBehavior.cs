namespace Stumps
{
    /// <summary>
    ///     An enumeration which defines the behavior of the <see cref="T:Stumps.SequencedHttpResponseFactory"/> class.
    /// </summary>
    public enum SequenceBehavior
    {
        /// <summary>
        /// Each incoming request returns the next response in the list sequentially.
        /// Once the cursor is beyond the current list of available responses, the cursor
        /// is reset to the beginning of the list.
        /// </summary>
        SequentialThenLoop = 0,

        /// <summary>
        /// Each incoming request returns the next response in the list sequentially.
        /// Once the cursor is beyond the current list of responses a default response is
        /// returned.
        /// </summary>
        SequentialThenDefault = 1,

        /// <summary>
        /// Each incoming request randomly chooses a response from the available list
        /// of responses.
        /// </summary>
        Random = 2
    }
}
