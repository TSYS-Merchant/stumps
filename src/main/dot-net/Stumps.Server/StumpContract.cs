namespace Stumps.Server
{
    /// <summary>
    ///     A class that represents a contract used to create a new Stump.
    /// </summary>
    public class StumpContract
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpContract"/> class.
        /// </summary>
        public StumpContract()
        {
            this.Rules = new RuleContractCollection();
        }

        /// <summary>
        ///     Gets or sets the original HTTP request used when editing the Stump.
        /// </summary>
        /// <value>
        ///     The original HTTP request used when editing the Stump.
        /// </value>
        public RecordedRequest OriginalRequest
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the original HTTP response to the Stump.
        /// </summary>
        /// <value>
        ///     The original HTTP response to the Stump.
        /// </value>
        public RecordedResponse OriginalResponse
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the response to the Stump.
        /// </summary>
        /// <value>
        ///     The response to the Stump.
        /// </value>
        public RecordedResponse Response
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the rules associated with the Stump.
        /// </summary>
        /// <value>
        ///     The rules associated with the Stump.
        /// </value>
        public RuleContractCollection Rules
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the organizational category the stump belongs to.
        /// </summary>
        /// <value>
        ///     The organizational category the stump belongs to.
        /// </value>
        public string StumpCategory
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the unique identifier for the stump.
        /// </summary>
        /// <value>
        ///     Gets or sets the unique identifier for the stump.
        /// </value>
        public string StumpId
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the stump.
        /// </summary>
        /// <value>
        ///     The name of the stump.
        /// </value>
        public string StumpName
        {
            get;
            set;
        }
    }
}