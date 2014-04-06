namespace Stumps.Server.Data
{

    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents the persisted form for a <see cref="T:Stumps.Stump"/>.
    /// </summary>
    public class StumpEntity
    {

        /// <summary>
        ///     Gets or sets the response to the Stump.
        /// </summary>
        /// <value>
        ///     The response to the Stump.
        /// </value>
        public HttpResponseEntity Response { get; set; }

        /// <summary>
        ///     Gets or sets an HTTP request used as a reference when editing a Stump.
        /// </summary>
        /// <value>
        ///     The HTTP request used as a reference when editing a Stump.
        /// </value>
        public HttpRequestEntity Request { get; set; }

        /// <summary>
        ///     Gets or sets the rules associated with the Stump.
        /// </summary>
        /// <value>
        ///     The rules associated with the Stump.
        /// </value>
        public List<RuleEntity> Rules { get; set; }

        /// <summary>
        ///     Gets or sets the organizational category the stump belongs to.
        /// </summary>
        /// <value>
        ///     The organizational category the stump belongs to.
        /// </value>
        public string StumpCategory { get; set; }

        /// <summary>
        ///     Gets or sets the unique identifier for the stump.
        /// </summary>
        /// <value>
        ///     Gets or sets the unique identifier for the stump.
        /// </value>
        public string StumpId { get; set; }

        /// <summary>
        ///     Gets or sets the name of the stump.
        /// </summary>
        /// <value>
        ///     The name of the stump.
        /// </value>
        public string StumpName { get; set; }

    }

}