namespace Stumps.Server
{
    /// <summary>
    ///     Defines the behavior of the server instance when recording traffic.
    /// </summary>
    public enum RecordingBehavior
    {
        /// <summary>
        ///     Disable stumps when recording.
        /// </summary>
        DisableStumps = 0,

        /// <summary>
        ///     Leave the status of the stumps unchanged.
        /// </summary>
        LeaveStumpsUnchanged = 1
    }
}
