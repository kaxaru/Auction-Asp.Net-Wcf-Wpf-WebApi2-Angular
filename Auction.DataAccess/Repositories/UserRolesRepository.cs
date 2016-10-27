using System;
using System.Collections.Generic;
using System.Linq;
using Auction.DataAccess.Database;
using Auction.DataAccess.DbConfig;
using Auction.DataAccess.Models;

namespace Auction.DataAccess.Repositories
{
    public class UserRolesRepository : IUserRolesRepository
    {
        private readonly IDatabaseConfig _dbconfig;
        private readonly IAuctionProvider _auctionProvider;
        private IStorage _storage;

        public UserRolesRepository(IDatabaseConfig dbconfig, IAuctionProvider auctionProvider)
        {
            _dbconfig = dbconfig;
            _auctionProvider = auctionProvider;
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
            _storage.SetModel(new Permission());
        }

        public void AddPermission(Permission permission)
        {
            /*Permission permission = new Permission()
            {
                AuctionId = permissionDTO.AuctionId,
                CategoriesId = permissionDTO.CategoriesId,
                Role = (int)permissionDTO.Role,
                Id = permissionDTO.Id,
                UserId = permissionDTO.UserId
            };*/

            _storage.AddAsync(permission);
            _storage.SaveChanges();
        }

        public void AddUserToRole(Guid userId, Role role)
        {
            throw new NotImplementedException();
        }

        public void EditPermission(Permission permission)
        {
            _storage.UpdateAsync(permission);
        }

        public IEnumerable<Permission> GetPermissions()
        {
            var permissions = _storage.QueryAsync<Permission>().Result;
            /*var permissionsDTO = new List<Permission>();
            foreach (var permission in permissions)
            {
                permissionsDTO.Add(new Permission()
                {
                    AuctionId = permission.AuctionId,
                    CategoriesId = permission.CategoriesId,
                    Id = permission.Id,
                    Role = (Role)permission.Role,
                    UserId = permission.UserId
                });
            }*/

            return permissions;
        }

        public void RemovePermission(Permission permission)
        {
            _storage.DeleteAsync(permission.Id);
        }

        public IEnumerable<Permission> GetUserRole(Guid userID)
        {
            var permissions = GetPermissions();
            return permissions.Where(p => p.UserId == userID);
        }
    }
}
