using System.Collections.Generic;
using Auction.Presentation.Models;

namespace Auction.Presentation.Areas.Admin.Models
{
    public class UsersWithPermissions
    {
       public IEnumerable<AdminUserViewModel> Users { get; set; }

       public IEnumerable<PermissionViewModel> Permissions { get; set; }
    }
}