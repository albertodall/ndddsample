namespace NDDDSample.Domain.Model.Voyages
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Validations;
    using Locations;
    using Shared;

    /// <summary>
    /// A Voyage - journey to some distant place,
    /// usually represents ocean trip as an act of traveling by water.
    /// </summary>
    public class Voyage : Entity<int>
    {
        // Null object pattern
        public static readonly Voyage NONE = new Voyage(new VoyageNumber(""), Schedule.EMPTY);

        private readonly Schedule schedule;
        private readonly VoyageNumber voyageNumber;

        #region Nested Voyage Builder 

        /// <summary>
        ///  Builder pattern is used for incremental construction
        ///  of a Voyage aggregate. This serves as an aggregate factory.        
        /// </summary>
        public class Builder
        {
            private readonly IList<CarrierMovement> carrierMovements = new List<CarrierMovement>();
            private readonly VoyageNumber voyageNumber;
            private Location departureLocation;

            public Builder(VoyageNumber voyageNumber, Location departureLocation)
            {
                Validate.NotNull(voyageNumber, "Voyage number is required");
                Validate.NotNull(departureLocation, "Departure location is required");

                this.voyageNumber = voyageNumber;
                this.departureLocation = departureLocation;
            }

            public Builder AddMovement(Location arrivalLocation, DateTime departureTime, DateTime arrivalTime)
            {
                carrierMovements.Add(new CarrierMovement(departureLocation, arrivalLocation, departureTime, arrivalTime));
                // Next departure location is the same as this arrival location
                departureLocation = arrivalLocation;
                return this;
            }

            public Voyage Build()
            {
                return new Voyage(voyageNumber, new Schedule(carrierMovements));
            }
        }

        #endregion

        protected Voyage()
        {
            // Needed by Hibernate
        }

        public Voyage(VoyageNumber voyageNumber, Schedule schedule)
        {
            Validate.NotNull(voyageNumber, "Voyage number is required");
            Validate.NotNull(schedule, "Schedule is required");

            this.voyageNumber = voyageNumber;
            this.schedule = schedule;
        }

        /// <summary>
        /// Voyage number.
        /// </summary>
        public virtual VoyageNumber VoyageNumber
        {
            get { return voyageNumber; }
        }

        /// <summary>
        /// GetSchedule
        /// </summary>
        /// <returns></returns>
        public virtual Schedule Schedule
        {
            get { return schedule; }
        }

        public override String ToString()
        {
            return $"Voyage {voyageNumber}";
        }
    }
}