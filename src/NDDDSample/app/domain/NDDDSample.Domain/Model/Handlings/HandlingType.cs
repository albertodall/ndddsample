namespace NDDDSample.Domain.Model.Handlings
{
    using Shared;

    /// <summary>
    ///Handling event type. Either requires or prohibits a carrier movement
    ///association, it's never optional.
    /// </summary>
    public class HandlingType : Enumeration
    {
        public static readonly HandlingType CLAIM = new HandlingType("CLAIM", false);
        public static readonly HandlingType CUSTOMS = new HandlingType("CUSTOMS", false);
        public static readonly HandlingType LOAD = new HandlingType("LOAD", true);
        public static readonly HandlingType RECEIVE = new HandlingType("RECEIVE", false);
        public static readonly HandlingType UNLOAD = new HandlingType("UNLOAD", true);

        private readonly bool voyageRequired;

        /// <summary>
        /// Private enum constructor        
        /// </summary>
        /// <param name="name">Enum string name</param>
        /// <param name="voyageRequired">voyageRequired whether or not a voyage is associated with this event type </param>
        /// TODO: See if it is possible NOT to make it public
        public HandlingType(string name, bool voyageRequired)
            : base(name)
        {
            this.voyageRequired = voyageRequired;
        }

        /// <summary>
        /// return True if a voyage association is required for this event type.
        /// </summary>
        /// <returns></returns>
        public bool RequiresVoyage()
        {
            return voyageRequired;
        }

        /// <summary>
        /// return True if a voyage association is prohibited for this event type.
        /// </summary>
        /// <returns></returns>
        public bool ProhibitsVoyage()
        {
            return !RequiresVoyage();
        }
    }
}