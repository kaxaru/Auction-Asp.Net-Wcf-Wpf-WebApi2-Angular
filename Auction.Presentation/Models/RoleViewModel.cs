using Auction.Presentation.Infrastructure.Filters;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public enum Role
    {
        [LocalizedDescriptionFilter("_role_user", typeof(Resource))]
        User = 1,
        [LocalizedDescriptionFilter("_role_moderator", typeof(Resource))]
        Moderator,
        [LocalizedDescriptionFilter("_role_admin", typeof(Resource))]
        Admin
    }
}