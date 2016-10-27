using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Areas.Admin.Models;
using Auction.Presentation.Helpers;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Localization;
using Auction.Presentation.Models;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class UserController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly CustomUserManager _customUserManager;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;

        public UserController(DataAccess.Repositories.IAuctionProvider auctionProvider, CustomUserManager customUserManager)
        {
            _auctionProvider = auctionProvider;
            _customUserManager = customUserManager;
        }

        public static async Task<string> GetCategories(string auctionName, List<Guid> categoryList)
        {
            if (categoryList.Count == 0)
            {
                return string.Empty;
            }

            var auction = config.AuctionHouses.Search(auctionName);
            Auctions.SetAuction(auction);
            IUnityContainer container = UnityConfig.GetConfiguredContainer();
            var categoryService = container.Resolve<ICategoriesService>();
            StringBuilder sb = new StringBuilder();
            foreach (var categoryId in categoryList)
            {
                var category = await categoryService.GetCategoryAsync(categoryId);
                sb.Append(category.Name).Append(";");
            }

            return sb.ToString();
        }

        public async Task<ActionResult> Index()
        {
            var users = await _customUserManager.GetAllUsersAsync();
            var usersVm = users.Select(el => Mapper.Map<AdminUserViewModel>(el));

            var permissions = await _customUserManager.GetAllPermissionsAsync();
            var permissionsVM = new List<Models.PermissionViewModel>();
            foreach (var permission in permissions)
            {
                permissionsVM.Add(new Models.PermissionViewModel()
                {
                    AuctionId = permission.AuctionId,
                    CategoriesId = permission.CategoriesId,
                    Role = (Presentation.Models.Role)(int)permission.Role,
                    UserId = permission.UserId
                });
            }

            ////var globalAdmin
            var globalPermissionAdmin = permissionsVM.Where(p => p.Role == Presentation.Models.Role.Admin);
            var adminUsers = new List<AdminUserViewModel>();
            foreach (var permission in globalPermissionAdmin)
            {
                var currentUser = usersVm.FirstOrDefault(u => u.Id == permission.UserId);
                if (currentUser != null)
                {
                    adminUsers.Add(currentUser);
                }
            }
            
            var usersDictionary = new Dictionary<string, UsersWithPermissions>();
            usersDictionary.Add(
                "admin", 
                new UsersWithPermissions()
            {
                Users = adminUsers,
                Permissions = globalPermissionAdmin
            });

            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                var auctionUsers = new List<AdminUserViewModel>();
                var auctionPermissions = new List<Models.PermissionViewModel>();
                foreach (Models.PermissionViewModel permissionVM in permissionsVM)
                {
                    if (permissionVM.AuctionId == auction.Name)
                    {
                        auctionPermissions.Add(permissionVM);
                    }
                }

                foreach (var auctionPermission in auctionPermissions)
                {
                    var currentUser = usersVm.FirstOrDefault(u => u.Id == auctionPermission.UserId);
                    if (currentUser != null)
                    {
                        auctionUsers.Add(currentUser);
                    }
                }

                usersDictionary.Add(
                    auction.Name, 
                    new UsersWithPermissions()
                {
                    Permissions = auctionPermissions,
                    Users = auctionUsers
                });
            }

                return View(usersDictionary);
        }

        public ActionResult Create()
        {
            Dropdown();
            AuctionList();
            return View();
        }       
    
        [HttpPost]
        public async Task<JsonResult> CreateUser(string data)
        { 
            dynamic user = new ExpandoObject();
            if (Request.IsAjaxRequest())
            {
                try
                {
                    user = JsonConvert.DeserializeObject(data);
                    string userModel = user.userWithPermissions.userModel.ToString();
                    var dataNvc = HttpUtility.ParseQueryString(userModel);
                    var dataCollection = dataNvc.AllKeys.ToDictionary(o => o, o => dataNvc[o]);
                    var jsonString = JsonConvert.SerializeObject(dataCollection);
                    AdminUserViewModel userVM = JsonConvert.DeserializeObject<AdminUserViewModel>(jsonString);
                    var locale = LocalizationService.GetAvalaibleLocalization();
                    if ((userVM.Locale == null) || (locale.FirstOrDefault(l => l.LocalizationId == userVM.Locale) == null))
                    {
                        ModelState.AddModelError("Locale", Resource.errLocaleNotFound);
                    }

                    if (userVM.TimezoneId == null || userVM.TimezoneId == string.Empty)
                    {
                        ModelState.AddModelError("TimezoneId", Resource.errTimeZoneNotFound);
                    }

                    if (!await _customUserManager.IsUniqueLoginAsync(userVM.Login))
                    {
                        ModelState.AddModelError("Login", Resource.errNotUnique);
                    }

                    userVM.Password = SecurityHelper.Hash(userVM.Password);                   
                    if (ModelState.IsValid)
                    {
                        string pic = user.userWithPermissions.file.ToString();
                        userVM.Picture = pic;

                        List<PermissionDTO> permissions = new List<PermissionDTO>();

                        foreach (dynamic permission in (IEnumerable<dynamic>)user.userWithPermissions.permissions)
                        {
                            var permissionDTO = new PermissionDTO() 
                            {      
                                Id = Guid.NewGuid(),                                                       
                                UserId = Guid.Parse((string)permission.userId),
                                Role = (Auction.BussinessLogic.Models.Role)Enum.Parse(typeof(Auction.BussinessLogic.Models.Role), (string)permission.role)
                            };

                            if (permissionDTO.Role != Auction.BussinessLogic.Models.Role.Admin)
                            {
                                permissionDTO.AuctionId = (string)permission.auctionName;
                                if (permissionDTO.Role == BussinessLogic.Models.Role.Moderator)
                                {
                                    permissionDTO.CategoriesId = new List<Guid>();
                                    foreach (dynamic key in (IEnumerable<dynamic>)permission.categories)
                                    {
                                            permissionDTO.CategoriesId.Add(Guid.Parse((string)key.Value));
                                    }
                                }
                                else
                                {
                                    permissionDTO.CategoriesId = null;
                                }
                            }

                            permissions.Add(permissionDTO);
                        }
                  
                        var userDTO = new UserDTO() { Id = permissions.First().UserId };
                        userVM.Id = userDTO.Id;
                        userVM.RegistrationDate = DateTime.Now;
                        userDTO.InjectFrom(userVM);

                        foreach (var permission in permissions)
                        {
                            await _customUserManager.AddPermissionAsync(permission);
                        }

                        await _customUserManager.AddUserAsync(userDTO);
                        return Json(JsonConvert.SerializeObject(Url.Action("Index", "User")));  
                    }
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, Resource.errCreate);
                }
            }

            return Json(JsonConvert.SerializeObject(Url.Action("Create", "User")));
        }

        public async Task<ActionResult> Edit(Guid? id)
        {         
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userDTO = await _customUserManager.FindByIdAsync(id.ToString());
            if (userDTO == null)
            {
                return HttpNotFound();
            }

            Dropdown();
            AuctionList();
            var userVM = Mapper.Map<AdminUserViewModel>(userDTO);
            var listPermissions = await _customUserManager.GetPermissionsAsync(userVM.Id);
            List<Models.PermissionViewModel> listPermissionsVM = new List<Models.PermissionViewModel>();
            foreach (var permissionDTO in listPermissions)
            {
                var permissionsVM = new Models.PermissionViewModel()
                {
                    UserId = permissionDTO.UserId,
                    Role = (Presentation.Models.Role)(int)permissionDTO.Role,
                    AuctionId = permissionDTO.AuctionId,
                    CategoriesId = new List<Guid>()
                };
                permissionsVM.CategoriesId = permissionDTO.CategoriesId;
                listPermissionsVM.Add(permissionsVM);
            }

            userVM.Password = string.Empty;
            var userWithPermissions = new UserWithPermissions()
            {
                User = userVM,
                Permissions = listPermissionsVM
            };

            return View(userWithPermissions);
        }

        [HttpPost]
        public async Task<JsonResult> EditUser(string data)
        {
            dynamic user = new ExpandoObject();
            if (Request.IsAjaxRequest())
            {
                try
                {
                    user = JsonConvert.DeserializeObject(data);
                    string userModel = user.userWithPermissions.userModel.ToString();
                    var dataNvc = HttpUtility.ParseQueryString(userModel);
                    var dataCollection = dataNvc.AllKeys.ToDictionary(o => o, o => dataNvc[o]);
                    var jsonString = JsonConvert.SerializeObject(dataCollection);
                    AdminUserViewModel userVM = JsonConvert.DeserializeObject<AdminUserViewModel>(jsonString);
                    var locale = LocalizationService.GetAvalaibleLocalization();
                    if ((userVM.Locale == null) || (locale.FirstOrDefault(l => l.LocalizationId == userVM.Locale) == null))
                    {
                        ModelState.AddModelError("Locale", Resource.errLocaleNotFound);
                    }

                    if (userVM.TimezoneId == null || userVM.TimezoneId == string.Empty)
                    {
                        ModelState.AddModelError("TimezoneId", Resource.errTimeZoneNotFound);
                    }

                    userVM.Password = SecurityHelper.Hash(userVM.Password);
                    if (ModelState.IsValid)
                    {
                        string pic = user.userWithPermissions.file.ToString();
                        userVM.Picture = pic;

                        var userPermissions = (IEnumerable<dynamic>)user.userWithPermissions.permissions;
                        var userId = Guid.Parse((string)userPermissions.First().userId);
                        UserViewModel currUser = null;
                        if (userId != Guid.Empty)
                        {
                            currUser = await _customUserManager.FindByIdAsync(userId.ToString());
                        }

                        if (currUser == null || (!currUser.UserName.Equals(userVM.Login) && !await _customUserManager.IsUniqueLoginAsync(userVM.Login)))
                        { 
                            ModelState.AddModelError("Login", Resource.errNotUnique);
                            return Json(JsonConvert.SerializeObject(Url.Action("Edit", "User")));
                        }

                        List<PermissionDTO> permissions = new List<PermissionDTO>();

                        foreach (dynamic permission in (IEnumerable<dynamic>)user.userWithPermissions.permissions)
                        {
                            var permissionDTO = new PermissionDTO()
                            {
                                Id = Guid.NewGuid(),
                                UserId = Guid.Parse((string)permission.userId),
                                Role = (Auction.BussinessLogic.Models.Role)Enum.Parse(typeof(Auction.BussinessLogic.Models.Role), (string)permission.role)
                            };

                            if (permissionDTO.Role != Auction.BussinessLogic.Models.Role.Admin)
                            {
                                permissionDTO.AuctionId = (string)permission.auctionName;
                                if (permissionDTO.Role == BussinessLogic.Models.Role.Moderator)
                                {
                                    permissionDTO.CategoriesId = new List<Guid>();
                                    foreach (dynamic key in (IEnumerable<dynamic>)permission.categories)
                                    {
                                        permissionDTO.CategoriesId.Add(Guid.Parse((string)key.Value));
                                    }
                                }
                                else
                                {
                                    permissionDTO.CategoriesId = null;
                                }
                            }

                            permissions.Add(permissionDTO);
                        }

                        var userDTO = new UserDTO() { Id = userId };
                        userVM.Id = userDTO.Id;
                        userVM.RegistrationDate = DateTime.Now;
                        userDTO.InjectFrom(userVM);

                        var listOldPermissions = await _customUserManager.GetPermissionsAsync(Guid.Parse(currUser.Id)); 
                        foreach (var oldPermission in listOldPermissions)
                        {
                            await _customUserManager.RemovePermissionAsync(oldPermission);
                        }

                        foreach (var permission in permissions)
                        {
                            await _customUserManager.AddPermissionAsync(permission);                            
                        }

                        await _customUserManager.EditUserAsync(userDTO);
                        return Json(JsonConvert.SerializeObject(Url.Action("Index", "User")));
                    }
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, Resource.errCreate);
                }
            }

            return Json(JsonConvert.SerializeObject(Url.Action("Edit", "User")));
        }

        public async Task<ActionResult> Delete(Guid? id, string auctionId, bool? saveChangeError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangeError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Произошла ошибка при удалении.";
            }

            var userDTO = await _customUserManager.FindByIdAsync(id.ToString());          
            if (userDTO == null)
            {
                return HttpNotFound();
            }

            var userVM = Mapper.Map<AdminUserViewModel>(userDTO);
            return View(userVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid? id)
        {
            try
            {
                var user = await _customUserManager.FindByIdAsync(id.ToString());
                if (user != null)
                {
                    var permissions = await _customUserManager.GetPermissionsAsync(id); 
                    foreach (var permission in permissions)
                    {
                        await _customUserManager.RemovePermissionAsync(permission);     
                    }

                   UserDTO userDTO = Mapper.Map<UserDTO>(user);
                   userDTO.RegistrationDate = user.RegistrationDate;
                   await _customUserManager.RemoveUserAsync(userDTO);
                }             
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        private void Dropdown()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            ViewBag.TimeZone = timeZones.Select(tz => new
            {
                timezoneId = tz.Id,
                Value = tz.DisplayName
            });           
        }

        private void AuctionList()
        {
            List<Models.AuctionViewModel> auctions = new List<Models.AuctionViewModel>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                var type = (Models.TypeAuctionVM)Enum.Parse(typeof(Models.TypeAuctionVM), auction.Type);
                var auctionVM = new Models.AuctionViewModel
                {
                    Name = auction.Name,
                    Location = auction.Location,
                    Type = type
                };
                auctions.Add(auctionVM);
            }

            ViewBag.Auctions = auctions.Select(auc => new
            {
                auctionId = auc.Name,
                Value = auc.Name,
            });
        }
    }
}