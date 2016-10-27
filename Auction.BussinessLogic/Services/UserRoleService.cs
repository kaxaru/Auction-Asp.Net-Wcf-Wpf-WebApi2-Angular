using System;
using System.Collections.Generic;
using System.Linq;
using Auction.BussinessLogic.Models;
using Auction.DataAccess.Models;
using Auction.DataAccess.Repositories;
using Omu.ValueInjecter;

namespace Auction.BussinessLogic.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRolesRepository _userRoleRepo;

        public UserRoleService(IUserRolesRepository userRoleRepo)
        {
            _userRoleRepo = userRoleRepo;
        }

        public IEnumerable<PermissionDTO> GetPermissions(Guid? userId)
        {
            _userRoleRepo.Configure();
            var permission = _userRoleRepo.GetUserRole(userId.GetValueOrDefault());
            return permission.Select(p => Mapper.Map<PermissionDTO>(p));
        }

        public IEnumerable<PermissionDTO> GetAllPermissions()
        {
            _userRoleRepo.Configure();
            var permissions = _userRoleRepo.GetPermissions();
            return permissions.Select(p => Mapper.Map<PermissionDTO>(p));         
        }

        public void AddPermission(PermissionDTO permissionDTO)
        {
            _userRoleRepo.Configure();
            if (_userRoleRepo == null)
            {
                throw new ArgumentException(nameof(_userRoleRepo));
            }

            var permission = new Permission() { };
            permission.InjectFrom(permissionDTO);
            _userRoleRepo.AddPermission(permission);
        }

        public void RemovePermission(PermissionDTO permissionDTO)
        {
            var permission = new Permission() { };
            permission.InjectFrom(permissionDTO);
            _userRoleRepo.RemovePermission(permission);
        }
    }
}
