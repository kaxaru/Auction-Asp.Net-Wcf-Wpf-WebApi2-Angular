using System;
using System.Collections.Generic;
using Auction.BussinessLogic.Models;

namespace Auction.BussinessLogic.Services
{
    public interface IUserRoleService
    {
        IEnumerable<PermissionDTO> GetPermissions(Guid? userId);

        IEnumerable<PermissionDTO> GetAllPermissions();

        void AddPermission(PermissionDTO permission);

        void RemovePermission(PermissionDTO permission);
    }
}
