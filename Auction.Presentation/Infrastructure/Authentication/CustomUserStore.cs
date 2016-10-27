using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Models;
using Auction.Presentation.RemoteStorage;
using Microsoft.AspNet.Identity;

namespace Auction.Presentation.Infrastructure.Authentication
{
    public class CustomUserStore : IUserPasswordStore<UserViewModel>, IUserClaimStore<UserViewModel>, IUserStore<UserViewModel>
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly Storage _userRepository;
        private readonly Storage _userRoleRepository;
        private JsonServiceClient jsonService = new JsonServiceClient();

        public CustomUserStore(Storage userRepository, Storage userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }

        public Task<string> GetPasswordHashAsync(UserViewModel user)
        {
            var stored = (User)jsonService.GetByIdAsync(_userRepository, Guid.Parse(user.Id)).Result;
            return Task.FromResult(new PasswordHasher().HashPassword(stored.Password));
        }

        public Task SetPasswordHashAsync(UserViewModel user, string passwordHash)
        {
            throw new NotImplementedException();
        }
      
        public Task<bool> HasPasswordAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public async Task AddAsync(JsonModel model)
        {
            await Task.Run(async () =>
            {
                switch (model.GetType().Name)
                {
                    case "User":
                        await jsonService.AddAsync(_userRepository, model);    
                        break;
                    case "Permission":
                        await jsonService.AddAsync(_userRoleRepository, model);
                        break;                   
                }
            });
        }

        public async Task<IEnumerable<JsonModel>> QueryAsync(JsonModel model)
        {
            Task<IEnumerable<JsonModel>> taskInvoke = Task<IEnumerable<JsonModel>>.Factory.StartNew(() =>
            {
                switch (model.GetType().Name)
                {
                    case "User":
                        var users = jsonService.QueryAsync(_userRepository).Result.Cast<User>();
                        return users;
                    case "Permission":
                        var permissions = jsonService.QueryAsync(_userRoleRepository).Result.Cast<Permission>();
                        return permissions;
                    default:
                        return new List<JsonModel>();
                }
            });
            return await taskInvoke;
        }

        public Task UpdateAsync(UserViewModel model)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(JsonModel model)
        {
            await Task.Run(async () =>
            {
                switch (model.GetType().Name)
                {
                    case "User":
                        await jsonService.UpdateAsync(_userRepository, model);
                        break;
                    case "Permission":
                        await jsonService.UpdateAsync(_userRoleRepository, model);
                        break;
                }
            });
        }

        public async Task RemoveAsync(JsonModel model)
        {
            await Task.Run(async () =>
            {
                switch (model.GetType().Name)
                {
                    case "User":
                        await jsonService.DeleteAsync(_userRepository, model.Id);
                        break;
                    case "Permission":
                        await jsonService.DeleteAsync(_userRoleRepository, model.Id);
                        break;
                }
            });
        }

        public Task DeleteAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public async Task<UserViewModel> FindByIdAsync(string userId)
        {
            return await Task<UserViewModel>.Factory.StartNew(() =>
            {
                var user = (User)jsonService.GetByIdAsync(_userRepository, Guid.Parse(userId)).Result;
                var userVM = new UserViewModel()
                {
                    Id = user.Id.ToString(),
                    UserName = user.Login,
                    Password = user.Password,
                    Locale = user.Locale,
                    TimezoneId = user.TimezoneId,
                    Picture = user.Picture,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RegistrationDate = user.RegistrationDate
                };
                return userVM;
            });
        }

        public async Task<JsonModel> FindByIdAsync(JsonModel model)
        {
             Task<JsonModel> taskInvoke = Task<JsonModel>.Factory.StartNew(() => 
             {
                switch (model.GetType().Name)
                {
                    case "User":
                        var user = (RemoteStorage.User)jsonService.GetByIdAsync(_userRepository, model.Id).Result;
                        return user;
                    case "Permission":
                        var permission = (RemoteStorage.Permission)jsonService.GetByIdAsync(_userRoleRepository, model.Id).Result;
                        return permission;
                    default:
                        return new JsonModel();
                }
            });
            return await taskInvoke;
        }

        public async Task<UserViewModel> FindByNameAsync(string userName)
        {
            Task<UserViewModel> taskInvoke = Task<UserViewModel>.Factory.StartNew(() =>
            {
                var users = jsonService.QueryAsync(_userRepository).Result.Cast<User>();
                var user = users.FirstOrDefault(u => u.Login == userName);
                if (user == null)
                {
                    return null;
                }

                return new UserViewModel()
                {
                    Id = user.Id.ToString(),
                    UserName = user.Login,
                    Password = user.Password,
                    Locale = user.Locale,
                    TimezoneId = user.TimezoneId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    RegistrationDate = user.RegistrationDate,
                    Picture = user.Picture
                };
            });            
            return await taskInvoke;
        }

        public void Dispose()
        {
        }

        public Task<IList<Claim>> GetClaimsAsync(UserViewModel user)
        {
            var listUserPermissions = jsonService.QueryAsync(_userRoleRepository).Result.Cast<Permission>();
            listUserPermissions = listUserPermissions.Where(p => p.UserId == Guid.Parse(user.Id));   
            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(@"http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/IdentityProvider", "ASP.NET Identity"));

            if (listUserPermissions.Count() == 1)
            {
                var role = (Role)listUserPermissions.First().Role;
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            else
            {
                foreach (AuctionHouseElement auction in config.AuctionHouses)
                {
                    var permission = listUserPermissions.FirstOrDefault(p => p.AuctionId == auction.Name);
                    if (permission != null)
                    {
                        var role = (Role)permission.Role;
                        claims.Add(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/" + auction.Name, role.ToString()));
                    }
                }
            }

            return Task.FromResult(claims);
        }

        public Task AddClaimAsync(UserViewModel user, Claim claim)
        {
            return Task.Run(() =>
            {
                var listClaims = GetClaimsAsync(user);
                listClaims.Result.Add(claim);
            });
        }

        public Task RemoveClaimAsync(UserViewModel user, Claim claim)
        {
            return Task.Run(() =>
            {
                var listClaims = GetClaimsAsync(user);
                listClaims.Result.Remove(claim);
            });
        }

        public Task AddToRoleAsync(UserViewModel user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(UserViewModel user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(UserViewModel user)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsInRoleAsync(UserViewModel user, string roleName)
        {
            throw new NotImplementedException();
        }
    }
}