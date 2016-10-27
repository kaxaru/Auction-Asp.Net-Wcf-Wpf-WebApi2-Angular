using System.Collections.Generic;
using Auction.Presentation.Models;

namespace Auction.Presentation.Areas.Admin.Models
{
    public class UserWithPermissions
    {
        public AdminUserViewModel User { get; set; }

        public IEnumerable<PermissionViewModel> Permissions { get; set; }
    }
}