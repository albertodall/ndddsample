namespace NDDDSample.Domain.Model.Locations
{
    using Infrastructure.Validations;
    using Shared;

    /// <summary>
    /// A location is our model is stops on a journey, such as cargo
    /// origin or destination, or carrier movement endpoints.
    /// 
    /// It is uniquely identified by a UN Locode.
    /// </summary>
    public class Location : Entity<int>
    {
        public static readonly Location UNKNOWN = new Location(new UnLocode("XXXXX"), "Unknown location");

        private readonly string name;
        private readonly UnLocode unLocode;

        /// <summary>
        ///  Package-level constructor, visible for test only.
        /// </summary>
        /// <param name="unLocode"> UN Locode</param>
        /// <param name="name">location name</param>
        /// TODO: See if it is possible NOT to make it public
        public Location(UnLocode unLocode, string name)
        {
            Validate.NotNull(unLocode);
            Validate.NotNull(name);

            this.unLocode = unLocode;
            this.name = name;
        }

        protected Location()
        {
            // Needed by Hibernate
        }

        public override string ToString()
        {
            return $"{name} [{unLocode}]";
        }

        /// <summary>
        /// Voyage UN Locode for this location.
        /// </summary>
        public virtual UnLocode UnLocode
        {
            get { return unLocode; }
        }

        /// <summary>
        /// Actual name of this location, e.g. "Stockholm".
        /// </summary>
        /// <returns></returns>
        public virtual string Name
        {
            get { return name; }
        }
    }
}