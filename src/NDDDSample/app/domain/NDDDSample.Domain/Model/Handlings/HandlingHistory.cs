namespace NDDDSample.Domain.Model.Handlings
{
    using System.Collections.Generic;
    using Infrastructure.Utils;
    using Infrastructure.Validations;

    /// <summary>
    /// The handling history of a cargo.
    /// </summary>
    public class HandlingHistory
    {
        public static readonly HandlingHistory EMPTY = new HandlingHistory(new List<HandlingEvent>());
        private readonly IList<HandlingEvent> handlingEvents;

        public HandlingHistory(IEnumerable<HandlingEvent> handlingEvents)
        {
            Validate.NotNull(handlingEvents, "Handling events are required");

            this.handlingEvents = new List<HandlingEvent>(handlingEvents);
        }


        /// <summary>
        /// A distinct list (no duplicate registrations) of handling events, ordered by completion time.
        /// </summary>
        /// <returns></returns>
        public IList<HandlingEvent> DistinctEventsByCompletionTime()
        {
            var ordered = new List<HandlingEvent>(new HashSet<HandlingEvent>(handlingEvents));

            ordered.Sort((he1, he2) => he1.CompletionTime.CompareTo(he2.CompletionTime));

            return new List<HandlingEvent>(ordered).AsReadOnly();
        }

        /// <summary>
        ///  Most recently completed event, or null if the delivery history is empty.
        /// </summary>
        /// <returns></returns>
        public HandlingEvent MostRecentlyCompletedEvent()
        {
            IList<HandlingEvent> distinctEvents = DistinctEventsByCompletionTime();
            if (distinctEvents.IsEmpty())
            {
                return null;
            }
            return distinctEvents[distinctEvents.Count - 1];
        }
    }
}