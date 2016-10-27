using System;
using System.Collections.Generic;
using System.IO;
using Auction.DataAccess.Helpers;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Data
{
    public static class DataInit
    {
        private static DataBinding db = new DataBinding();

        static DataInit()
        {
           // DataBinding db = new DataBinding();
            db.Categories = new List<Category>()
            {
                new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Rings",
                    Description = "Different rings"                   
                },
                new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Earnings",
                    Description = "Different earnings"
                },
                new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Watches",
                    Description = "Different watches"
                },
                new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Bracelets",
                    Description = "Different bracelets"
                },
                new Category()
                {
                    Id = Guid.NewGuid(),
                    Name = "Necklaces",
                    Description = "Different necklaces"
                }
            };

            var productModel = typeof(Product).Name;

            List<Product> productList = new List<Product>();

            foreach (var category in db.Categories)
            {
                productList.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    CategoryID = category.Id,
                    StartDate = DateTime.Now + TimeSpan.FromDays(5),
                    Duration = TimeSpan.FromHours(3).ToString(),
                    State = (int)State.Selling,
                    Name = "FAY PAY JEWELS",
                    Description = "FAY PAY JEWELS Brand New Ring With 2.68ctw Opals",
                    StartPrice = 1,
                    Picture = ImageHelper.LoadImage(Path.Combine("Auction1", productModel, category.Name, "1.jpg"))
                });

                productList.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    CategoryID = category.Id,
                    StartDate = DateTime.Now + TimeSpan.FromDays(6),
                    Duration = TimeSpan.FromHours(7).ToString(),
                    State = (int)State.Selling,
                    Name = "Magnolia",
                    Description = "Brand New Ring With Genuine",
                    StartPrice = 2,
                    Picture = ImageHelper.LoadImage(Path.Combine("Auction1", productModel, category.Name, "2.jpg"))
                });

                productList.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    CategoryID = category.Id,
                    StartDate = DateTime.Now + TimeSpan.FromDays(7),
                    Duration = TimeSpan.FromHours(4).ToString(),
                    State = (int)State.Selling,
                    Name = "KREMENTZ",
                    Description = "KREMENTZ USA since 1866 Brand New Ring With 1.51ctw",
                    StartPrice = 5,
                    Picture = ImageHelper.LoadImage(Path.Combine("Auction1", productModel, category.Name, "3.jpg"))
                });

                productList.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    CategoryID = category.Id,
                    StartDate = DateTime.Now + TimeSpan.FromDays(3),
                    Duration = TimeSpan.FromHours(2).ToString(),
                    State = (int)State.Selling,
                    Name = "VIDA",
                    Description = "Brand New Ring With 1.20ctw Precious Stones ",
                    StartPrice = 2,
                    Picture = ImageHelper.LoadImage(Path.Combine("Auction1", productModel, category.Name, "4.jpg"))
                });

                productList.Add(new Product()
                {
                    Id = Guid.NewGuid(),
                    CategoryID = category.Id,
                    StartDate = DateTime.Now + TimeSpan.FromDays(2),
                    Duration = TimeSpan.FromHours(10).ToString(),
                    State = (int)State.Selling,
                    Name = "AURORA BOREALIS",
                    Description = "AURORA BOREALIS Brand New Ring With Cubic zirconia 925.",
                    StartPrice = 3,
                    Picture = ImageHelper.LoadImage(Path.Combine("Auction1", productModel, category.Name, "5.jpg"))
                });                 
            }

            db.Products = productList;       
            var timeZones = TimeZoneInfo.GetSystemTimeZones();
            var timezoneCount = timeZones.Count;
            var rnd = new Random();
            var user = typeof(User).Name;
            var defDir = Path.Combine(user, "cat.png");      
            db.Users = new List<User>()
            {
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "admin",
                    Password = SecurityHelper.Hash("admin"),
                    FirstName = "admin",
                    LastName = "admin",
                    RegistrationDate = DateTime.Now,
                    Locale = "ru-Ru",
                    TimezoneId = timeZones[rnd.Next(1, timezoneCount)].Id,
                    Picture = ImageHelper.LoadImage(defDir)
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "moderator1",
                    Password = SecurityHelper.Hash("moderator1"),
                    FirstName = "moderator1",
                    LastName = "moderator1",
                    RegistrationDate = DateTime.Now,
                    Locale = "be-By",
                    TimezoneId = timeZones[rnd.Next(1, timezoneCount)].Id,
                    Picture = ImageHelper.LoadImage(defDir)
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "moderator2",
                    Password = SecurityHelper.Hash("moderator2"),
                    FirstName = "moderator2",
                    LastName = "moderator2",
                    RegistrationDate = DateTime.Now,
                    Locale = "be-By",
                    TimezoneId = timeZones[rnd.Next(1, timezoneCount)].Id,
                    Picture = ImageHelper.LoadImage(defDir)
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "moderator3",
                    Password = SecurityHelper.Hash("moderator3"),
                    FirstName = "moderator3",
                    LastName = "moderator3",
                    RegistrationDate = DateTime.Now,
                    Locale = "be-By",
                    TimezoneId = timeZones[rnd.Next(1, timezoneCount)].Id,
                    Picture = ImageHelper.LoadImage(defDir)
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "user",
                    Password = SecurityHelper.Hash("user"),
                    FirstName = "user",
                    LastName = "user",
                    RegistrationDate = DateTime.Now,
                    Locale = "en-Us",
                    TimezoneId = timeZones[rnd.Next(1, timezoneCount)].Id,
                    Picture = ImageHelper.LoadImage(defDir)
                }
            };    
        }

        public static DataBinding GetData()
        {
            return db;
        }
    }
}
