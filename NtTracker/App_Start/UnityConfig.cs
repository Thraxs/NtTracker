using System;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using NtTracker.Data;
using NtTracker.Services;

namespace NtTracker
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            //Authentication manager
            container.RegisterType<IAuthenticationManager>(
                new InjectionFactory(f => HttpContext.Current.GetOwinContext().Authentication));

            //Database context
            container.RegisterType<DataContext>(new PerRequestLifetimeManager());

            //Services
            container.RegisterType<IService, Service>(new PerRequestLifetimeManager());
            container.RegisterType<IUserAccountService, UserAccountService>(new PerRequestLifetimeManager());
            container.RegisterType<IOperationService, OperationService>(new PerRequestLifetimeManager());
            container.RegisterType<IPatientService, PatientService>(new PerRequestLifetimeManager());
            container.RegisterType<INbrSurveillanceService, NbrSurveillanceService>(new PerRequestLifetimeManager());
            container.RegisterType<ICnsExplorationService, CnsExplorationService>(new PerRequestLifetimeManager());
            container.RegisterType<IMonitoringService, MonitoringService>(new PerRequestLifetimeManager());
            container.RegisterType<IHypothermiaService, HypothermiaService>(new PerRequestLifetimeManager());
            container.RegisterType<IAnalysisService, AnalysisService>(new PerRequestLifetimeManager());
        }
    }
}
