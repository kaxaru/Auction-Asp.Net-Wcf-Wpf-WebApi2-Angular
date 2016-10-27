using System;
using System.Collections.Generic;
using Auction.DataAccess.Database;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public interface IUserRolesRepository
    {
        IStorage Storage { get; }

        IEnumerable<Permission> GetPermissions();

        void AddPermission(Permission permission);

        void RemovePermission(Permission permission);

        void EditPermission(Permission permission);

        void AddUserToRole(Guid userId, Role role);

        IEnumerable<Permission> GetUserRole(Guid userID);

        void Configure();
    }
}
