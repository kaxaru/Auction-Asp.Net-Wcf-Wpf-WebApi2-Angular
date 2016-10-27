using System;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Auction.BussinessLogic.Services;
using Auction.DataAccess.DatabaseContext;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.JSONParsing;
using Auction.DataAccess.Repositories;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Models;
using Auction.Presentation.RemoteStorage;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Unity.WebApi;

namespace Auction.Presentation
{
    public static class UnityConfig
    {
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            var folder = HostingEnvironment.MapPath("~/App_Data/");

            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserRepository, JSONUserRepository>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IUserRolesRepository, UserRolesRepository>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<IProductRepository, JSONProductRepository>();

            container.RegisterType<ICategoriesService, CategoryService>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IJsonStorage, JsonStorage>(new InjectionConstructor(folder));
            container.RegisterType<ISqlStorage, AuctionDbContext>(new InjectionConstructor(folder));
            container.RegisterType<DataAccess.Repositories.IAuctionProvider, DataAccess.Repositories.AuctionProvider>();
            container.RegisterType<IDatabaseConfig, DatabaseConfig>();

            JsonServiceClient jsonService = new JsonServiceClient();
            var userJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.Model = typeof(DataAccess.Models.User).Name;
            var userRoleJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.Permission().ToString());
            userRoleJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.Permission().ToString());
            userRoleJson.Model = typeof(DataAccess.Models.Permission).Name;

            container.RegisterType<IUserStore<UserViewModel, string>, CustomUserStore>(new InjectionConstructor(userJson, userRoleJson));
            container.RegisterType<IBidRepository, BidRepoditory>();
            container.RegisterType<IBidService, BidService>();

            ////container.RegisterType<UserManager<UserViewModel, string>>();
            //// container.RegisterType<UserManager<UserViewModel>, CustomUserManager>();
        }

        public static void RegisterComponents()
        {
            var container = new UnityContainer();
            var folder = HostingEnvironment.MapPath("~/App_Data/");

            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserRepository, JSONUserRepository>();
            container.RegisterType<IUserRoleService, UserRoleService>();
            container.RegisterType<IUserRolesRepository, UserRolesRepository>();
            container.RegisterType<IProductService, ProductService>();
            container.RegisterType<IProductRepository, JSONProductRepository>();

            container.RegisterType<ICategoriesService, CategoryService>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IJsonStorage, JsonStorage>(new InjectionConstructor(folder));
            container.RegisterType<ISqlStorage, AuctionDbContext>(new InjectionConstructor(folder));
            container.RegisterType<DataAccess.Repositories.IAuctionProvider, DataAccess.Repositories.AuctionProvider>();
            container.RegisterType<IDatabaseConfig, DatabaseConfig>();

            JsonServiceClient jsonService = new JsonServiceClient();
            var userJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.Model = typeof(DataAccess.Models.User).Name;
            var userRoleJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.Permission().ToString());
            userRoleJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.Permission().ToString());
            userRoleJson.Model = typeof(DataAccess.Models.Permission).Name;
            container.RegisterType<IUserStore<UserViewModel, string>, CustomUserStore>(new InjectionConstructor(userJson, userRoleJson));
            container.RegisterType<IBidRepository, BidRepoditory>();
            container.RegisterType<IBidService, BidService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}