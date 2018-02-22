namespace NDDDSample.Domain.Model.Cargos
{
    using System;
    using Infrastructure.Validations;
    using Locations;
    using Shared;
    using Voyages;

    /// <summary>
    /// An itinerary consists of one or more legs.
    /// </summary>
    public class Leg : ValueObject<Leg>
    {
        public Location LoadLocation { get; private set; }
        public DateTime LoadTime { get; private set; }
        public Location UnloadLocation { get; private set; }
        public DateTime UnloadTime { get; private set; }
        public Voyage   Voyage { get; private set; }
         
        public Leg(Voyage voyage, Location loadLocation, Location unloadLocation, DateTime loadTime, DateTime unloadTime)
        {
            Validate.NoNullElements(new object[] {voyage, loadLocation, unloadLocation, loadTime, unloadTime});

            Voyage = voyage;
            LoadLocation = loadLocation;
            UnloadLocation = unloadLocation;
            LoadTime = loadTime;
            UnloadTime = unloadTime;

            RegisterProperty(p => p.Voyage);
            RegisterProperty(p => p.LoadLocation);
            RegisterProperty(p => p.UnloadLocation);
            RegisterProperty(p => p.LoadTime);
            RegisterProperty(p => p.UnloadTime);
        }
    }
}