﻿namespace NDDDSample.Domain.Model.Cargos
{
    #region Usings

    using Shared;

    #endregion

    /// <summary>
    /// Routing status. 
    /// </summary>
    public class RoutingStatus : Enumeration
    {
        public static readonly RoutingStatus MISROUTED = new RoutingStatus("MISROUTED");
        public static readonly RoutingStatus NOT_ROUTED = new RoutingStatus("NOT_ROUTED");
        public static readonly RoutingStatus ROUTED = new RoutingStatus("ROUTED");

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Enum string name</param>
        private RoutingStatus(string name)
            : base(name) {}
    }
}