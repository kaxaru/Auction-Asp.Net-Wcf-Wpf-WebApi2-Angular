using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction.BussinessLogic.Infrastructure
{
    public static class IocConfigure
    {
        public static void ConfigureIocUnityContainer()
        {
            IUnityContainer container = new UnityContainer();
            registerServices(container);

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static void registerServices(IUnityContainer container)
        {
            container.RegisterType<IUserRepository, JSONUserRepository>();
            container.RegisterType<IProductRepository, JSONProductRepository>();
            container.RegisterType<ICategoryRepository, JSONCategoryRepository>();
            container.RegisterType<ICategoriesService, CategoryService>();
            container.RegisterType<IProductService, IProductService>();
            //container.RegisterType<IUserStore<UserViewModel>, CustomUserStore>();
            container.RegisterType<UserManager<UserViewModel>>();
        }
    }
}
