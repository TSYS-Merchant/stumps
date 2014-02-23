namespace Stumps.Http
{

    using System;

    /// <summary>
    ///     Provides data for an event that occurred for a <see cref="T:Stumps.Http.StumpsHttpContext"/>.
    /// </summary>
    internal sealed class StumpsContextEventArgs : EventArgs
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.Http.StumpsContextEventArgs"/> class.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Http.StumpsHttpContext"/> associated with the event.</param>
        public StumpsContextEventArgs(StumpsHttpContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the <see cref="T:Stumps.Http.StumpsHttpContext"/> associated with the event.
        /// </summary>
        /// <value>
        /// The <see cref="T:Stumps.Http.StumpsHttpContext"/> associated with the event.
        /// </value>
        public StumpsHttpContext Context { get; private set; }

    }

}