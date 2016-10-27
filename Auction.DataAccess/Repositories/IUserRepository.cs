using System;
using System.Collections.Generic;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public interface IUserRepository
    {
        IStorage Storage { get; }

        IEnumerable<User> GetUsers();

        User GetById(Guid id);

        void AddUser(User user);

        void RemoveUser(User user);

        void UpdateUser(User user);

        void Configure();
    }
}
