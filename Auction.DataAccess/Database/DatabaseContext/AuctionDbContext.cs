using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.DatabaseContext
{
    public class AuctionDbContext : DbContext, ISqlStorage, IStorage
    {
        private static string _folder;
        private Type modelType;
        private DbSet data;

        public AuctionDbContext(string folder)
        {
            _folder = folder;
        }

        public AuctionDbContext(string folder, string location) : base(ConnectionString(folder, location))
        {
            if (!IsFileExists(location))
            {
                System.Data.Entity.Database.SetInitializer<AuctionDbContext>(new CreateDatabaseIfNotExists<AuctionDbContext>());
            }          
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Bid> Bids { get; set; }

        public void Initial()
        {
            System.Data.Entity.Database.SetInitializer<AuctionDbContext>(new AuctionDbInit());
            Database.Initialize(true);
        }

        public void GetConnectionString(string connection)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync<T>(Guid id) where T : JsonModel
        {
            return await Task<T>.Run(() => data.Find(id) as T);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>() where T : JsonModel
        {
            return await Task<IEnumerable<T>>.Run(() =>
            {
                var data = Set(modelType);
                return data.OfType<T>();
            });
        }

        public async Task AddAsync<T>(T item) where T : JsonModel
        {
            await Task.Run(() => data.Add(item));         
        }

        public async Task UpdateAsync<T>(T item) where T : JsonModel
        {
            await Task.Run(() =>
            {
                JsonModel element;
                switch (modelType.Name)
                {
                    case "Category":
                        element = data.Cast<Category>().SingleOrDefault(el => el.Id == item.Id);
                        break;
                    case "Product":
                        element = data.Cast<Product>().SingleOrDefault(el => el.Id == item.Id);
                        break;
                    case "Bid":
                        element = data.Cast<Bid>().SingleOrDefault(el => el.Id == item.Id);
                        break;
                    default:
                        element = null;
                        break;
                }

                if (element == null)
                {
                    throw new Exception();
                }

                element = item;
            });                
        }

        public async Task DeleteAsync(Guid id)
        {
           await Task.Run(() =>
            {
                JsonModel element;
                switch (modelType.Name)
                {
                    case "Category":
                        element = data.Cast<Category>().SingleOrDefault(el => el.Id == id);
                        break;
                    case "Product":
                        element = data.Cast<Product>().SingleOrDefault(el => el.Id == id);
                        break;
                    case "Bid":
                        element = data.Cast<Bid>().SingleOrDefault(el => el.Id == id);
                        break;
                    default:
                        element = null;
                        break;
                }

                if (element == null)
                {
                    throw new Exception();
                }

                data.Remove(element);
            });         
        }

        async Task IStorage.SaveChanges()
        {
           await Task.Run(() => SaveChanges());
        }

        public IStorage SetConnectionString(string connection)
        {
            return new AuctionDbContext(null, connection);
        }

        public void SetModel(JsonModel modelForSwitch)
        {
            modelType = modelForSwitch.GetType();
            data = Set(modelType);
        }

        public bool IsFileExists(string filePath)
        {
            var location = Path.Combine(_folder, filePath);
            return File.Exists(location);
        }

        private static string ConnectionString(string folder, string location)
        {
            if (folder != null)
            {
                _folder = folder;
            }

            location = Path.Combine(_folder, location);
            return string.Format(@"Data Source = (LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security = True; Connect Timeout = 30", location);
        }
    }
}
