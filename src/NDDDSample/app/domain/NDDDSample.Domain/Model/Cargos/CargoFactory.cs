namespace NDDDSample.Domain.Model.Cargos
{
    using System;
    using Locations;

    /// <summary>
    /// Cargo factory.
    /// The purpouse of the factory that is hides internal strcuture of the cargo.    
    /// </summary>
    public static class CargoFactory
    {
        public static Cargo NewCargo(TrackingId trackingId, Location origin, Location destination, DateTime arrivalDeadline)
        {           
            var routeSpecification = new RouteSpecification(origin, destination, arrivalDeadline);
            return new Cargo(trackingId, routeSpecification);
        }
    }
}