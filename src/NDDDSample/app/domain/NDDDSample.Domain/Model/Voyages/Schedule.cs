namespace NDDDSample.Domain.Model.Voyages
{
    using System.Collections.Generic;
    using Infrastructure.Validations;

    /// <summary>
    /// A voyage schedule.
    /// </summary>
    public class Schedule
    {
        public static readonly Schedule EMPTY = new Schedule(new List<CarrierMovement>());
        private readonly IList<CarrierMovement> carrierMovements = new List<CarrierMovement>();

        public Schedule(IList<CarrierMovement> carrierMovements)
        {
            Validate.NotNull(carrierMovements);
            Validate.NoNullElements(carrierMovements);
            Validate.NotEmpty(carrierMovements);

            this.carrierMovements = new List<CarrierMovement>(carrierMovements);
        }

        /// <summary>
        /// Carrier movements.
        /// </summary>
        public IList<CarrierMovement> CarrierMovements
        {
            get { return new List<CarrierMovement>(carrierMovements).AsReadOnly(); }
        }
    }
}