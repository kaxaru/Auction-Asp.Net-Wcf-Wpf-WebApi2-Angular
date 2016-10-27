using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Hosting;
using Auction.BussinessLogic.Models;
using Auction.Presentation.Models;
using Auction.Presentation.RemoteStorage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Omu.ValueInjecter;

namespace Auction.Presentation.Infrastructure.Authentication
{
    public class CustomUserManager : UserManager<UserViewModel>
    {
        private static CustomUserStore _store;

        public CustomUserManager(CustomUserStore store) : base(store)
        {            
            _store = store;
            this.PasswordHasher = new CustomPasswordHasher();
        }

        public static CustomUserManager Create(IdentityFactoryOptions<CustomUserManager> options,
                                            IOwinContext context)
        {
            var folder = HostingEnvironment.MapPath("~/App_Data/");
            JsonServiceClient jsonService = new JsonServiceClient();
            var userJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.User().ToString());
            userJson.Model = typeof(DataAccess.Models.User).Name;
            var userRoleJson = jsonService.CreateJson(folder, new Auction.DataAccess.Models.Permission().ToString());  
            userRoleJson.FilePath = Path.Combine(folder, new Auction.DataAccess.Models.Permission().ToString());
            userRoleJson.Model = typeof(DataAccess.Models.Permission).Name;
            CustomUserManager manager = new CustomUserManager(new CustomUserStore(userJson, userRoleJson));
            return manager;
        }

        public override async Task<UserViewModel> FindAsync(string userName, string password)
        {  
            Task<UserViewModel> taskInvoke = Task<UserViewModel>.Factory.StartNew(() =>
            {
                var user = _store.FindByNameAsync(userName).Result;
                if (user != null)
                {
                    var providedPassword = this.PasswordHasher.HashPassword(password);
                    PasswordVerificationResult result = this.PasswordHasher.VerifyHashedPassword(user.Password, providedPassword);
                    if (result == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        return user;
                    }

                    return null;
                }
                else
                {
                    return null;
                }
            });
            return await taskInvoke;
        }

        public override async Task<UserViewModel> FindByIdAsync(string userId)
        {
            Task<UserViewModel> taskInvoke = Task<UserViewModel>.Factory.StartNew(() =>
            {
                var user = _store.FindByIdAsync(userId).Result;
                return user;
            });

            return await taskInvoke;
        }

        public override async Task<IdentityResult> AddClaimAsync(string userId, Claim claim)
        {
            Task<IdentityResult> taskInvoke = Task<IdentityResult>.Factory.StartNew(() =>
            {
                var user = _store.FindByIdAsync(userId).Result;              
                return _store.AddClaimAsync(user, claim).IsCompleted ? IdentityResult.Success : IdentityResult.Failed();
            });

            return await taskInvoke;
        }

        public override Task<ClaimsIdentity> CreateIdentityAsync(UserViewModel user, string autType)
        {
            var listClaim = _store.GetClaimsAsync(user).Result;

            var claimsIdentity = new ClaimsIdentity(listClaim, DefaultAuthenticationTypes.ApplicationCookie);

            return Task.Run(() => claimsIdentity);
        }

        public async Task<bool> IsUniqueLoginAsync(string login)
        {
            Task<bool> taskInvoke = Task<bool>.Factory.StartNew(() =>
            {
                var user = _store.FindByNameAsync(login).Result;
                return user == null ? true : false;
            });

            return await taskInvoke;
        }

        public async Task AddUserAsync(UserDTO userDTO)
        {
            await Task.Factory.StartNew(async () =>
            {
                var user = Mapper.Map<RemoteStorage.User>(userDTO);
                await _store.AddAsync(user);
            });
        }

        public async Task EditUserAsync(UserDTO userDTO)
        {
            await Task.Factory.StartNew(async () =>
            {
                var user = await _store.FindByIdAsync(userDTO.Id.ToString());
                var regDate = user.RegistrationDate;
                user.InjectFrom<NoNullsInjection>(userDTO);
                RemoteStorage.User userRemote = Mapper.Map<RemoteStorage.User>(user);
                userRemote.Id = Guid.Parse(user.Id);
                userRemote.Login = user.UserName;
                userRemote.RegistrationDate = regDate;
                await _store.UpdateAsync(userRemote);
            });
        }

        public async Task RemoveUserAsync(UserDTO userDTO)
        {
            await Task.Factory.StartNew(async () =>
            {              
                RemoteStorage.User user = Mapper.Map<RemoteStorage.User>(userDTO);
                user.RegistrationDate = userDTO.RegistrationDate;
                await _store.RemoveAsync(user);
            });
        }

        public async Task AddPermissionAsync(PermissionDTO permissionDTO)
        {
            await Task.Factory.StartNew(async () =>
            {
                var permission = Mapper.Map<RemoteStorage.Permission>(permissionDTO);
                permission.Role = (int)permissionDTO.Role;
                permission.CategoriesId = permissionDTO.CategoriesId?.ToArray();
                await _store.AddAsync(permission);
            });
        }

        public async Task RemovePermissionAsync(PermissionDTO permissionDTO)
        {
            await Task.Factory.StartNew(async () =>
            {
                RemoteStorage.Permission permission = Mapper.Map<RemoteStorage.Permission>(permissionDTO);
                await _store.RemoveAsync(permission);
            });
        }

        public async Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync()
        {
            Task<IEnumerable<PermissionDTO>> taskInvoke = Task<IEnumerable<PermissionDTO>>.Factory.StartNew(() =>
            {
               var permissions = _store.QueryAsync(new RemoteStorage.Permission()).Result;
                List<PermissionDTO> permissionsDTO = new List<PermissionDTO>();
                foreach (RemoteStorage.Permission permission in permissions)
                {
                    PermissionDTO permissionDTO = new PermissionDTO()
                    {
                        AuctionId = permission.AuctionId,
                        Id = permission.Id,
                        UserId = permission.UserId,
                        Role = (BussinessLogic.Models.Role)permission.Role,
                        CategoriesId = new List<Guid>()                     
                    };
                    permissionDTO.CategoriesId = permission.CategoriesId?.ToList();
                    permissionsDTO.Add(permissionDTO);
                };

               return permissionsDTO;
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<PermissionDTO>> GetPermissionsAsync(Guid? userId)
        {
            Task<IEnumerable<PermissionDTO>> taskInvoke = Task<IEnumerable<PermissionDTO>>.Factory.StartNew(() =>
            {
                var permissions = _store.QueryAsync(new RemoteStorage.Permission()).Result.Cast<RemoteStorage.Permission>();
                var userPermissions = permissions.Where(p => p.UserId == userId);
                List<PermissionDTO> permissionsDTO = new List<PermissionDTO>();
                foreach (var permission in userPermissions)
                {
                    PermissionDTO permissionDTO = new PermissionDTO()
                    {
                        AuctionId = permission.AuctionId,
                        Id = permission.Id,
                        UserId = permission.UserId,
                        Role = (BussinessLogic.Models.Role)permission.Role,
                        CategoriesId = new List<Guid>()
                    };

                    permissionDTO.CategoriesId = permission.CategoriesId.ToList();
                    permissionsDTO.Add(permissionDTO);
                }

                return permissionsDTO;
            });

            return await taskInvoke;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            Task<IEnumerable<UserDTO>> taskInvoke = Task<IEnumerable<UserDTO>>.Factory.StartNew(() =>
            {
                var users = _store.QueryAsync(new RemoteStorage.User()).Result;
                List<UserDTO> usersDTO = new List<UserDTO>();
                foreach (RemoteStorage.User user in users)
                {
                    UserDTO userDTO = Mapper.Map<UserDTO>(user);
                    userDTO.RegistrationDate = user.RegistrationDate; 
                };

                return usersDTO;
            });

            return await taskInvoke;
        }        
    }
}