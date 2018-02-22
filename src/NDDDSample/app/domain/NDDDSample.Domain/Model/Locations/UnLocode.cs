namespace NDDDSample.Domain.Model.Locations
{
    using System.Text.RegularExpressions;
    using Infrastructure.Validations;
    using Shared;

    /// <summary>
    /// United nations location code.
    /// http://www.unece.org/cefact/locode/
    /// http://www.unece.org/cefact/locode/DocColumnDescription.htm#LOCODE</summary>
    public class UnLocode : ValueObject<UnLocode>
    {
        // Country code is exactly two letters.
        // Location code is usually three letters, but may contain the numbers 2-9 as well
        private static readonly Regex VALID_PATTERN = new Regex("[a-zA-Z]{2}[a-zA-Z2-9]{3}", RegexOptions.Compiled);

        public string Code { get; private set; }

        /// <summary>
        ///  Constructor.
        /// </summary>
        /// <param name="countryAndLocation">Location string</param>
        public UnLocode(string countryAndLocation)
        {
            Validate.NotNull(countryAndLocation, "Country and location may not be null");
            Validate.IsTrue(VALID_PATTERN.Match(countryAndLocation).Success,
                            countryAndLocation + " is not a valid UN/LOCODE (does not match pattern)");

            Code = countryAndLocation.ToUpper();
            RegisterProperty(p => p.Code);
        }

    }
}