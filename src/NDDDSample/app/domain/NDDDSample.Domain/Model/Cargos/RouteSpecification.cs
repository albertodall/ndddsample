namespace NDDDSample.Domain.Model.Cargos
{
    using System;
    using Infrastructure.Utils;
    using Infrastructure.Validations;
    using Locations;
    using Shared;

    /// <summary>
    ///  Route specification. Describes where a cargo orign and destination is,
    /// and the arrival deadline.
    /// </summary>
    public class RouteSpecification : AbstractSpecification<Itinerary>
    {
        public DateTime ArrivalDeadline { get; private set; }
        public Location Destination { get; private set; }
        public Location Origin { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="origin">origin location - can't be the same as the destination</param>
        /// <param name="destination">destination location - can't be the same as the origin</param>
        /// <param name="arrivalDeadline">arrival deadline</param>
        public RouteSpecification(Location origin, Location destination, DateTime arrivalDeadline)
        {
            Validate.NotNull(origin, "Origin is required");
            Validate.NotNull(destination, "Destination is required");
            Validate.NotNull(arrivalDeadline, "Arrival deadline is required");
            Validate.IsTrue(origin != destination, "Origin and destination can't be the same: " + origin);

            Origin = origin;
            Destination = destination;
            ArrivalDeadline = arrivalDeadline;
        }

        protected RouteSpecification()
        {
            // Needed by Hibernate
        }

        public override bool IsSatisfiedBy(Itinerary itinerary)
        {
            return itinerary != null &&
                   Origin == itinerary.InitialDepartureLocation &&
                   Destination == itinerary.FinalArrivalLocation &&
                   ArrivalDeadline.After(itinerary.FinalArrivalDate);
        }
    }
}