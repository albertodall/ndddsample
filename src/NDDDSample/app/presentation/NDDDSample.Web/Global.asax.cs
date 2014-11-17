namespace NDDDSample.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Routing;  
    using Controllers;
    using Initializers;

    using Microsoft.WindowsAzure.ServiceRuntime;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using System.Web;

    public class MvcApplication : HttpApplication, IContainerAccessor
    {
        private readonly IWindsorContainer _container = new WindsorContainer();

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeServiceLocator();
            RegisterRoutes(RouteTable.Routes);
            NhInitializer.Init();
        }

        protected void Application_End()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Instantiate the container and add all Controllers that derive from 
        /// WindsorController to the container.  Also associate the Controller 
        /// with the WindsorContainer ControllerFactory.
        /// </summary>
        protected virtual void InitializeServiceLocator()
        {
            _container.Register(Classes.FromThisAssembly().BasedOn<BaseController>().LifestyleTransient());

            RegisterDependencies();

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(_container));

            //TODO: Register repositories and services for controllers
           // ServiceLocator.SetLocatorProvider(() => new WindsorServiceLocator(Container));
        }

        private void RegisterDependencies()
        {
            bool isRunInTheCloud;
            try
            {
                isRunInTheCloud = RoleEnvironment.IsAvailable;
            }
            catch (Exception)
            {
                isRunInTheCloud = false;
            }

            if (isRunInTheCloud)
            {
                var current = RoleEnvironment.CurrentRoleInstance;
                

                var roleInstanceEndpoints = RoleEnvironment.Roles["BookingRemoteServiceWorkerRole"]
                    .Instances
                    .Where(instance => instance != current)
                    .Select(instance => instance.InstanceEndpoints["BookingRemoteServiceWorkerRoleEndpoint"]);

                var bookingInternalEndpoint = roleInstanceEndpoints.ElementAt(new Random().Next(roleInstanceEndpoints.Count())).IPEndpoint.ToString();
                                
                ComponentRegistrar.AddComponentsTo(this.Container, bookingInternalEndpoint);
            }
            else
            {
                ComponentRegistrar.AddComponentsTo(this.Container);
            }
        }

        private static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" }); 

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Home", action = "Index", id = ""} // Parameter defaults
                );
        }

        public IWindsorContainer Container
        {
            get 
            {
                lock (_container)
                {
                    return _container;
                }
            }
        }
    }
}