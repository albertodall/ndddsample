namespace NDDDSample.Web.Initializers
{
    using System;
    using System.ServiceModel;
    using Castle.Facilities.WcfIntegration;
    using Castle.Facilities.WcfIntegration.Behaviors;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using NDDDSample.Infrastructure.Utils;
    using NDDDSample.Interfaces.BookingRemoteService.Common;

    public static class ComponentRegistrar
    {
        private static string bookingRemoteServiceWorkerRoleEndpoint = "localhost:8081";

        public static void AddComponentsTo(IWindsorContainer container, string bookingEndpoint)
        {
            bookingRemoteServiceWorkerRoleEndpoint = bookingEndpoint;            
            AddComponentsTo(container);
        }

        public static void AddComponentsTo(IWindsorContainer container)
        {
            AddCustomRepositoriesTo(container);
        }

        private static void AddCustomRepositoriesTo(IWindsorContainer container)
        {
            container.Register(
                Classes
                    //Scan repository assembly for domain model interfaces implementation
                    .FromAssemblyNamed("NDDDSample.Persistence.NHibernate")
                    .InNamespace("NDDDSample.Persistence.NHibernate")
                    .WithServiceFirstInterface());
                    //.WithService.FirstNonGenericCoreInterface("NDDDSample.Domain.Model"));

            container.AddFacility<WcfFacility>();

            container.Register(
                Component.For<MessageLifecycleBehavior>(),                
                Component.For<IBookingServiceFacade>()                
                    .Named("bookingServiceFacade")
                    .LifeStyle.Transient
                    .AsWcfClient(DefaultClientModel
                        .On(WcfEndpoint.BoundTo(new NetTcpBinding())
                        .At(String.Format("net.tcp://{0}/BookingServiceFacade", bookingRemoteServiceWorkerRoleEndpoint))
                    )
            ));            
        }
    }
}