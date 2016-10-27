using Auction.Presentation.Helpers;
using Microsoft.AspNet.Identity;

namespace Auction.Presentation.Infrastructure.Authentication
{
    public class CustomPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return SecurityHelper.Hash(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(providedPassword))
            {
                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}