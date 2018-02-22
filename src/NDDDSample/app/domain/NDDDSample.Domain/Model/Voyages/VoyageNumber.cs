namespace NDDDSample.Domain.Model.Voyages
{
    using Infrastructure.Validations;
    using Shared;

    /// <summary>
    /// Identifies a voyage.
    /// </summary>
    public class VoyageNumber : ValueObject<VoyageNumber>
    {
        public string Number { get; private set; }

        public VoyageNumber(string number)
        {
            Validate.NotNull(number);

            Number = number;
            RegisterProperty(p => p.Number);
        }


        public string IdString
        {
            get { return Number; }
        }
    }
}