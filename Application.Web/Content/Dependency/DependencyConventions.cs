
using Application.Common.Logging;
using Application.Common.Validator;
using Application.Core.ProfileModule.ProfileAggregate;
using Application.DAL;
using Application.DAL.Contract;
using Application.Manager.Contract;
using Application.Manager.Implementation;
using Application.Repository.ProfileModule;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace Application.Web.Dependency
{
    public class DependencyConventions : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                                .BasedOn<IController>()
                                .LifestyleTransient());

            container.Register(

                        Component.For<IQueryableUnitOfWork, UnitOfWork>().ImplementedBy<UnitOfWork>(),

                        Component.For<IProfileRepository, ProfileRepository>().ImplementedBy<ProfileRepository>(),

                        Component.For<IContactManager>().ImplementedBy<ContactManager>(),

                        AllTypes.FromThisAssembly().BasedOn<IHttpController>().LifestyleTransient()

                        )
                       .AddFacility<LoggingFacility>(f => f.UseLog4Net());

            LoggerFactory.SetCurrent(new TraceSourceLogFactory());
            EntityValidatorFactory.SetCurrent(new DataAnnotationsEntityValidatorFactory());


        }
        
    }
}