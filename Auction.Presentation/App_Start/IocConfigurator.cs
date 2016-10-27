using System.Web.Hosting;
using System.Web.Mvc;
using Auction.DataAccess.JSONParsing;
using Auction.DataAccess.Repositories;
using Auction.Presentation.Infrastructure;
using Microsoft.Practices.Unity;

namespace Auction.Presentation.App_Start
{
    public static class IocConfigurator
    {
        public static void ConfigureIocUnityContainer()
        {
            IUnityContainer container = new UnityContainer();
            
            RegisterServices(container);
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static void RegisterServices(IUnityContainer container)
        {
            var map = HostingEnvironment.MapPath("~/App_Data/data.json");
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IJsonStorage, JsonStorage>(new InjectionConstructor(HostingEnvironment.MapPath("~/App_Data/data.json")));
        }
    }
}