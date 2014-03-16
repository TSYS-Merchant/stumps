namespace Stumps
{

    using System;

    /// <summary>
    ///     Provides data for an event that occurred for a <see cref="T:Stumps.IStumpsHttpContext"/>.
    /// </summary>
    internal sealed class StumpsContextEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.StumpsContextEventArgs"/> class.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.IStumpsHttpContext"/> associated with the event.</param>
        public StumpsContextEventArgs(IStumpsHttpContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the <see cref="T:Stumps.IStumpsHttpContext"/> associated with the event.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.IStumpsHttpContext"/> associated with the event.
        /// </value>
        public IStumpsHttpContext Context { get; private set; }

    }

}