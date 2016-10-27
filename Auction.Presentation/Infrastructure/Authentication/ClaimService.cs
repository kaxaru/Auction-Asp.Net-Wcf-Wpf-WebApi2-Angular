using System.Collections.Generic;

namespace Auction.Presentation.Infrastructure.Authentication
{
    public class ClaimService
    {
        private static IDictionary<string, string> _claims = new Dictionary<string, string>()
        {
            { "timezone", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/timezone" }
        };

        public static IDictionary<string, string> ClaimsType
        {
            get { return _claims; }        
        }
    }
}