namespace NDDDSample.Domain.Model.Voyages
{
    using System;
    using Infrastructure.Validations;
    using Locations;
    using Shared;

    /// <summary>
    /// A carrier movement is a vessel voyage from one location to another.
    /// </summary>
    public class CarrierMovement : ValueObject<CarrierMovement>
    {
        // Null object pattern 
        public static CarrierMovement NONE = new CarrierMovement(
            Location.UNKNOWN, Location.UNKNOWN, new DateTime(0), new DateTime(0));

        public Location ArrivalLocation { get; private set; }
        public DateTime ArrivalTime { get; private set; }
        public Location DepartureLocation { get; private set; }
        public DateTime DepartureTime { get; private set; }

        /// <summary>
        /// Constructor.
        /// TODO make assembly local
        /// </summary>
        /// <param name="departureLocation">location of departure</param>
        /// <param name="arrivalLocation">location of arrival</param>
        /// <param name="departureTime">time of departure</param>
        /// <param name="arrivalTime">time of arrival</param>
        public CarrierMovement(Location departureLocation,
                               Location arrivalLocation,
                               DateTime departureTime,
                               DateTime arrivalTime)
        {
            Validate.NoNullElements(new object[] {departureLocation, arrivalLocation, departureTime, arrivalTime});
            DepartureTime = departureTime;
            ArrivalTime = arrivalTime;
            DepartureLocation = departureLocation;
            ArrivalLocation = arrivalLocation;

            RegisterProperty(p => p.DepartureTime);
            RegisterProperty(p => p.ArrivalTime);
            RegisterProperty(p => p.DepartureLocation);
            RegisterProperty(p => p.ArrivalLocation);
        }
    }
}