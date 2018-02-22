namespace NDDDSample.Domain.Model.Cargos
{
    using Handlings;
    using Infrastructure.Validations;
    using Locations;
    using Shared;
    using Voyages;

    /// <summary>
    /// A handling activity represents how and where a cargo can be handled,
    /// and can be used to express predictions about what is expected to
    /// happen to a cargo in the future.
    /// </summary>
    public class HandlingActivity : ValueObject<HandlingActivity>
    {
        // TODO make HandlingActivity a part of HandlingEvent too? There is some overlap. 
        public Location Location { get; private set; }
        public HandlingType Type { get; private set; }
        public Voyage Voyage { get; private set; }


        public HandlingActivity(HandlingType type, Location location, Voyage voyage)
        {
            Validate.NotNull(type, "Handling event type is required");
            Validate.NotNull(location, "Location is required");
            Validate.NotNull(location, "Voyage is required");

            Type = type;
            Location = location;
            Voyage = voyage;

            RegisterProperty(p => p.Type);
            RegisterProperty(p => p.Location);
            RegisterProperty(p => p.Voyage);
        }
    }
}