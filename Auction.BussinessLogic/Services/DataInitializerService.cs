using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Auction.DataAccess.Data;
using Auction.DataAccess.DatabaseContext;
using Auction.DataAccess.JSONParsing;
using Auction.DataAccess.Models;

namespace Auction.BussinessLogic.Services
{
    public class DataInitializerService
    {
        private readonly string _folderMVC;
        private readonly List<Auction.BussinessLogic.Models.Auction> _auctions;
        private readonly DataBinding db;

        public DataInitializerService(string folderMVC, List<Auction.BussinessLogic.Models.Auction> auctions)
        {
            _auctions = auctions;
            _folderMVC = folderMVC;         
            db = DataInit.GetData();
        }

        public void Initialize()
        {
            UserInit();
            foreach (var auction in _auctions)
            { 
                switch (auction.Type)
                {
                    case Models.TypeAuction.Json:
                        this.CategoriesInitJSON(auction.Name, auction.Location);
                        this.ProductsInitJSON(auction.Name, auction.Location);
                        break;
                    case Models.TypeAuction.Sql:
                        this.InitSQL(auction.Name, auction.Location);                        
                        break;
                    default:
                        this.CategoriesInitJSON(auction.Name, auction.Location);
                        this.ProductsInitJSON(auction.Name, auction.Location);
                        break;
                }
            }

            PermissionInit();
        }

        public async void CategoriesInitJSON(string name, string location)
        {
            var storage = new JsonStorage(_folderMVC);
            storage.SetPathToFile(name);
            storage.SetModel(new Category());

            if (storage.QueryAsync<Category>().Result.Count() == 0)
            {
                foreach (var category in db.Categories)
                {
                    await storage.AddAsync(category);
                    await storage.SaveChanges();
                }
            }          
        }

        public async void ProductsInitJSON(string name, string location)
        {
            var storage = new JsonStorage(_folderMVC);
            storage.SetPathToFile(name);
            storage.SetModel(new Product());
            if (storage.QueryAsync<Product>().Result.Count() == 0)
            {
                foreach (var product in db.Products)
                {
                    await storage.AddAsync(product);
                    await storage.SaveChanges();
                }
            }                  
        }

        private async void UserInit()
        {
            var storage = new JsonStorage(_folderMVC);

            storage.SetModel(new User());

            if (storage.QueryAsync<User>().Result.Count() == 0)
            {
                foreach (var user in db.Users)
                {
                   await storage.AddAsync<User>(user);
                   await storage.SaveChanges();
                }
            }
        }

        private async void PermissionInit()
        {
            var storage = new JsonStorage(_folderMVC);
            storage.SetModel(new Permission());
            Random rnd = new Random();

            if (storage.QueryAsync<Permission>().Result.Count() == 0)
            {
                var users = db.Users;
                int num;

                foreach (var user in users)
                {
                    var id = Guid.NewGuid();
                    var userId = user.Id;
                    if (users.First().Id == user.Id)
                    {
                        await storage.AddAsync(new Permission()
                        {
                            Id = id,
                            UserId = userId,
                            Role = (int)Models.Role.Admin
                        });
                        await storage.SaveChanges();
                    }
                    else
                    {
                        foreach (var auction in _auctions)
                        {
                            var permission = new Permission();
                            permission.Id = id;
                            permission.UserId = user.Id;
                            num = rnd.Next(Enum.GetNames(typeof(Models.Role)).Length - 1);
                            permission.Role = (int)(Models.Role)(num + 1);
                            permission.AuctionId = auction.Name;
                            permission.CategoriesId = new List<Guid>();
                            if (permission.Role == (int)Models.Role.Moderator)
                            {
                                num = rnd.Next(db.Categories.Count);
                                for (var i = 0; i < num; i++)
                                {
                                    permission.CategoriesId.Add(db.Categories[i].Id);
                                }
                            }

                            await storage.AddAsync(permission);
                            await storage.SaveChanges();
                        }
                    }
                }
            }
        }

        private void InitSQL(string name, string location)
        {
            string filePath = Path.Combine(location, string.Format("{0}.{1}", name, "mdf"));
            AuctionDbContext db = new AuctionDbContext(_folderMVC, filePath);
            db.Initial();
        }
    }
}
