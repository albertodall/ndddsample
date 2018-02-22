namespace NDDDSample.Domain.Model.Cargos
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using Handlings;
    using Infrastructure.Builders;
    using Infrastructure.Validations;
    using Locations;
    using Shared;
    using Voyages;

    #endregion

    /// <summary>
    /// The actual transportation of the cargo, as opposed to
    /// the customer requirement (RouteSpecification) and the plan (Itinerary). 
    /// </summary>
    public class Delivery : ValueObject<Delivery>
    {
        //TODO: atrosin revise ETA_UNKOWN = null
        public static readonly DateTime ETA_UNKOWN = DateTime.MinValue;

        private static HandlingActivity NO_ACTIVITY;
        public DateTime CalculatedAt { get; private set; }
        public Voyage CurrentVoyage { get; private set; }
        public DateTime ETA { get; private set; }
        public bool IsUnloadedAtDestination { get; private set; }
        public HandlingEvent LastEvent { get; private set; }
        public Location LastKnownLocation { get; private set; }
        public bool IsMisdirected { get; private set; }
        public HandlingActivity NextExpectedActivity { get; private set; }
        public RoutingStatus RoutingStatus { get; private set; }
        public TransportStatus TransportStatus { get; private set; }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="lastEvent">last event</param>
        /// <param name="itinerary">itinerary</param>
        /// <param name="routeSpecification">route specification</param>
        public Delivery(HandlingEvent lastEvent, Itinerary itinerary, RouteSpecification routeSpecification)
        {
            NO_ACTIVITY = null;
            CalculatedAt = DateTime.Now;
            LastEvent = lastEvent;

            IsMisdirected = CalculateMisdirectionStatus(itinerary);
            RoutingStatus = CalculateRoutingStatus(itinerary, routeSpecification);
            TransportStatus = CalculateTransportStatus();
            LastKnownLocation = CalculateLastKnownLocation();
            CurrentVoyage = CalculateCurrentVoyage();
            ETA = CalculateEta(itinerary);
            NextExpectedActivity = CalculateNextExpectedActivity(routeSpecification, itinerary);
            IsUnloadedAtDestination = CalculateUnloadedAtDestination(routeSpecification);

            RegisterProperty(p => p.LastEvent);
            RegisterProperty(p => p.IsMisdirected);
            RegisterProperty(p => p.TransportStatus);
            RegisterProperty(p => p.LastKnownLocation);
            RegisterProperty(p => p.CurrentVoyage);
            RegisterProperty(p => p.ETA);
            RegisterProperty(p => p.NextExpectedActivity);
            RegisterProperty(p => p.IsUnloadedAtDestination);
        }

        /// <summary>
        /// Creates a new delivery snapshot to reflect changes in routing, i.e.
        /// when the route specification or the itinerary has changed
        /// but no additional handling of the cargo has been performed.
        /// </summary>
        /// <param name="routeSpecification">route specification</param>
        /// <param name="itinerary">itinerary itinerary</param>
        /// <returns>An up to date delivery</returns>
        internal Delivery UpdateOnRouting(RouteSpecification routeSpecification, Itinerary itinerary)
        {
            Validate.NotNull(routeSpecification, "Route specification is required");

            return new Delivery(LastEvent, itinerary, routeSpecification);
        }

        internal static Delivery DerivedFrom(RouteSpecification routeSpecification, Itinerary itinerary,
                                             HandlingHistory handlingHistory)
        {
            Validate.NotNull(routeSpecification, "Route specification is required");
            Validate.NotNull(handlingHistory, "Delivery history is required");

            HandlingEvent lastEvent = handlingHistory.MostRecentlyCompletedEvent();

            return new Delivery(lastEvent, itinerary, routeSpecification);
        }

        // --- Internal calculations below ---

        // TODO add currentCarrierMovement (?)
        private TransportStatus CalculateTransportStatus()
        {
            //TODO: atrosin revise the if pattern spagetti code

            if (LastEvent == null)
            {
                return TransportStatus.NOT_RECEIVED;
            }

            if (LastEvent.Type == HandlingType.LOAD)
            {
                return TransportStatus.ONBOARD_CARRIER;
            }

            bool isInPort = LastEvent.Type == HandlingType.UNLOAD
                            || LastEvent.Type == HandlingType.RECEIVE
                            || LastEvent.Type == HandlingType.CUSTOMS;

            if (isInPort)
            {
                return TransportStatus.IN_PORT;
            }

            if (LastEvent.Type == HandlingType.CLAIM)
            {
                return TransportStatus.CLAIMED;
            }

            return TransportStatus.UNKNOWN;
        }

        private Location CalculateLastKnownLocation()
        {
            if (LastEvent != null)
            {
                return LastEvent.Location;
            }
            return Location.UNKNOWN;
        }

        private Voyage CalculateCurrentVoyage()
        {
            if (TransportStatus == TransportStatus.ONBOARD_CARRIER && LastEvent != null)
            {
                return LastEvent.Voyage;
            }
            return Voyage.NONE;
        }

        private bool CalculateMisdirectionStatus(Itinerary itinerary)
        {
            if (LastEvent == null)
            {
                return false;
            }
            return !itinerary.IsExpected(LastEvent);
        }

        private DateTime CalculateEta(Itinerary itinerary)
        {
            if (IsOnTrack())
            {
                return itinerary.FinalArrivalDate;
            }

            return ETA_UNKOWN;
        }

        private HandlingActivity CalculateNextExpectedActivity(RouteSpecification routeSpecification,
                                                               Itinerary itinerary)
        {
            if (!IsOnTrack())
            {
                return NO_ACTIVITY;
            }

            if (LastEvent == null)
            {
                return new HandlingActivity(HandlingType.RECEIVE, routeSpecification.Origin, CurrentVoyage);
            }

            if (LastEvent.Type == HandlingType.LOAD)
            {
                foreach (Leg leg in itinerary.Legs)
                {
                    if (leg.LoadLocation == LastEvent.Location)
                    {
                        return new HandlingActivity(HandlingType.UNLOAD, leg.UnloadLocation,
                                                    leg.Voyage);
                    }
                }

                return NO_ACTIVITY;
            }

            if (LastEvent.Type == HandlingType.UNLOAD)
            {
                for (IEnumerator<Leg> it = itinerary.Legs.GetEnumerator(); it.MoveNext();)
                {
                    Leg leg = it.Current;
                    if (leg.UnloadLocation == LastEvent.Location)
                    {
                        if (it.MoveNext())
                        {
                            Leg nextLeg = it.Current;
                            return new HandlingActivity(HandlingType.LOAD, nextLeg.LoadLocation,
                                                        nextLeg.Voyage);
                        }
                        return new HandlingActivity(HandlingType.CLAIM, leg.UnloadLocation, CurrentVoyage);
                    }
                }
                return NO_ACTIVITY;
            }

            if (LastEvent.Type == HandlingType.RECEIVE)
            {
                IEnumerator<Leg> enumerator = itinerary.Legs.GetEnumerator();
                enumerator.MoveNext();
                var firstLeg = enumerator.Current;
                return new HandlingActivity(HandlingType.LOAD, firstLeg.LoadLocation, firstLeg.Voyage);
            }

            if (LastEvent.Type == HandlingType.CLAIM)
            {
                //DO nothing
            }

            return NO_ACTIVITY;
        }


        private RoutingStatus CalculateRoutingStatus(Itinerary itinerary, RouteSpecification routeSpecification)
        {
            if (itinerary == null)
            {
                return RoutingStatus.NOT_ROUTED;
            }

            if (routeSpecification.IsSatisfiedBy(itinerary))
            {
                return RoutingStatus.ROUTED;
            }

            return RoutingStatus.MISROUTED;
        }

        private bool CalculateUnloadedAtDestination(RouteSpecification routeSpecification)
        {
            return LastEvent != null &&
                   HandlingType.UNLOAD == LastEvent.Type &&
                   routeSpecification.Destination == LastEvent.Location;
        }

        private bool IsOnTrack()
        {
            return RoutingStatus == RoutingStatus.ROUTED && !IsMisdirected;
        }
    }
}