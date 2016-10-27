using System;
using System.Collections.Generic;
using Auction.DataAccess.Database;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public class JSONUserRepository : IUserRepository
    {
        private readonly IDatabaseConfig _dbconfig;
        private readonly IAuctionProvider _auctionProvider;
        private IStorage _storage;

        public JSONUserRepository(IDatabaseConfig dbconfig, IAuctionProvider auction)
        {
            _dbconfig = dbconfig;
            _auctionProvider = auction;
        }

        public IStorage Storage
        {
            get
            {
                return _storage;
            }
        }

        public void Configure()
        {
            _storage = _dbconfig.GetGlobalDatabase();
            _storage.SetModel(new User());
        }

        public IEnumerable<Models.User> GetUsers()
        {
            return _storage.QueryAsync<User>().Result;
        }

        public User GetById(Guid id)
        {
            var user = _storage.GetByIdAsync<User>(id).Result;
            return user;
        }

        public void AddUser(User user)
        {
            var jsonModelUser = new User { Id = Guid.NewGuid() };
            ////jsonModelUser.InjectFrom(user);
            _storage.AddAsync(user);
            _storage.SaveChanges();
        }

        public void RemoveUser(User user)
        {
            _storage.DeleteAsync(user.Id);
            _storage.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            var jsonModelUser = _storage.GetByIdAsync<User>(user.Id).Result;
            ////jsonModelUser.InjectFrom<NoNullsInjection>(user);
            _storage.UpdateAsync(jsonModelUser);
            _storage.SaveChanges();
        }
    }
}
