using Auction.Presentation.Infrastructure.Filters;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Models
{
    public class StateViewModel
    {
        public enum State
        {
            [LocalizedDescriptionFilter("_state_draft", typeof(Resource))]
            Draft,
            [LocalizedDescriptionFilter("_state_selling", typeof(Resource))]
            Selling,
            [LocalizedDescriptionFilter("_state_banned", typeof(Resource))]
            Banned,
            [LocalizedDescriptionFilter("_state_outdated", typeof(Resource))]
            OutDated
        }
    }
}