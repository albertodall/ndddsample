namespace NDDDSample.Domain.Model.Cargos
{
    using System;
    using System.Collections.Generic;
    using Handlings;
    using Infrastructure.Utils;
    using Infrastructure.Validations;
    using Locations;
    using Shared;

    /// <summary>
    /// An itinerary - plan, path: an established line of travel.
    /// </summary>
    public class Itinerary : ValueObject<Itinerary>
    {
        internal static readonly Itinerary EMPTY_ITINERARY = new Itinerary(new List<Leg>());
        private static readonly DateTime END_OF_DAYS = DateTime.MaxValue;
        private readonly IList<Leg> _legs;

        public Itinerary(IList<Leg> legs)
        {
            Validate.NotEmpty(legs);
            Validate.NoNullElements(legs);

            _legs = new List<Leg>(legs);
            RegisterProperty(p => p.Legs);
        }

        /// <summary>
        /// the legs of this itinerary, as an <b>immutable</b> list.
        /// </summary>
        public IList<Leg> Legs
        {
            get { return new List<Leg>(_legs).AsReadOnly(); }
        }

        /// <summary>
        /// The initial departure location.
        /// </summary>
        internal Location InitialDepartureLocation
        {
            get
            {
                if (_legs.IsEmpty())
                {
                    return Location.UNKNOWN;
                }
                return _legs[0].LoadLocation;
            }
        }

        /// <summary>
        /// Date when cargo arrives at final destination.
        /// </summary>
        internal Location FinalArrivalLocation
        {
            get
            {
                if (_legs.IsEmpty())
                {
                    return Location.UNKNOWN;
                }
                return LastLeg.UnloadLocation;
            }
        }

        /// <summary>
        /// Date when cargo arrives at final destination.
        /// </summary>
        internal DateTime FinalArrivalDate
        {
            get
            {
                Leg lastLeg = LastLeg;

                if (lastLeg == null)
                {
                    return END_OF_DAYS;
                }

                return lastLeg.UnloadTime;
            }
        }

        /// <summary>
        /// The last leg on the itinerary.
        /// </summary>
        private Leg LastLeg
        {
            get
            {
                if (_legs.IsEmpty())
                {
                    return null;
                }
                return _legs[_legs.Count - 1];
            }
        }

        /// <summary>
        ///  Test if the given handling event is expected when executing this itinerary.
        /// </summary>
        /// <param name="handlingEvent">event Event to test.</param>
        /// <returns>true if the event is expected</returns>
        public bool IsExpected(HandlingEvent handlingEvent)
        {
            //TODO: atrosin revise the logic if it is transl corectlly
            if (_legs.IsEmpty())
            {
                return true;
            }

            if (handlingEvent.Type == HandlingType.RECEIVE)
            {
                //Check that the first leg's origin is the event's location
                Leg leg = _legs[0];
                return leg.LoadLocation.Equals(handlingEvent.Location);
            }

            if (handlingEvent.Type == HandlingType.LOAD)
            {
                //Check that the there is one leg with same load location and voyage
                foreach (var leg in _legs)
                {
                    if (leg.LoadLocation == handlingEvent.Location &&
                        leg.Voyage == handlingEvent.Voyage)
                    {
                        return true;
                    }
                }
                return false;
            }

            if (handlingEvent.Type == HandlingType.UNLOAD)
            {
                //Check that the there is one leg with same unload location and voyage
                foreach (var leg in _legs)
                {
                    if (leg.UnloadLocation.Equals(handlingEvent.Location) &&
                        leg.Voyage.Equals(handlingEvent.Voyage))
                    {
                        return true;
                    }
                }
                return false;
            }

            if (handlingEvent.Type == HandlingType.CLAIM)
            {
                //Check that the last leg's destination is from the event's location
                Leg leg = LastLeg;
                return (leg.UnloadLocation.Equals(handlingEvent.Location));
            }

            //HandlingEvent.Type.CUSTOMS;
            return true;
        }
    }
}