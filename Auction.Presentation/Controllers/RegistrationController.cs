using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.Presentation.Helpers;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Localization;
using Auction.Presentation.Models;
using Microsoft.AspNet.Identity.Owin;
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    [AllowAnonymous]
    public class RegistrationController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private CustomUserManager _customUserManager;       

        public RegistrationController(CustomUserManager userManager)
        {
            _customUserManager = userManager;
        }

        public CustomUserManager UserManager
        {
            get
            {
                return _customUserManager ??
                       (_customUserManager = HttpContext.GetOwinContext().GetUserManager<CustomUserManager>());
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            Dropdown();
            return View(new RegistrationViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Index(RegistrationViewModel user, HttpPostedFileBase image, Guid id)
        {
            user.Id = id;
            var locale = LocalizationService.GetAvalaibleLocalization();
            if ((user.Locale == null) || (locale.FirstOrDefault(l => l.LocalizationId == user.Locale) == null))
            {
                ModelState.AddModelError("Locale", Resource.errLocaleNotFound);
            }

            if (user.TimezoneId == null || user.TimezoneId == string.Empty)
            {
                ModelState.AddModelError("TimezoneId", Resource.errTimeZoneNotFound);
            }

            if (!await _customUserManager.IsUniqueLoginAsync(user.Login))
            {
                ModelState.AddModelError("Login", Resource.errNotUnique);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    user.Password = Helpers.SecurityHelper.Hash(user.Password);
                    user.DPassword = Helpers.SecurityHelper.Hash(user.DPassword);
                    if (user.Password != user.DPassword)
                    {
                        ModelState.AddModelError(string.Empty, Resource.errDPassword);
                        throw new DataException();
                    }

                    if (image != null)
                    {
                        string extension = System.IO.Path.GetExtension(image.FileName);
                        if ((extension == ".jpg") || (extension == ".png"))
                        {
                            var imageBase64 = ImageHelper.LoadImage(image);
                            user.Picture = imageBase64;
                        }
                        else
                        {
                            ModelState.AddModelError("Picture", Resource.errPictureFormat);
                        }
                    }

                    user.RegistrationDate = DateTime.Now;
                    var regUser = Mapper.Map<Auction.BussinessLogic.Models.UserDTO>(user);
                    ////default Role = 0 - User 
                    await _customUserManager.AddUserAsync(regUser);
                    List<Task> taskList = new List<Task>();
                    foreach (AuctionHouseElement auctionEl in config.AuctionHouses)
                    {
                        PermissionDTO permissionDTO = new PermissionDTO()
                        {
                            AuctionId = auctionEl.Name,
                            UserId = regUser.Id,
                            Id = Guid.NewGuid(),
                            CategoriesId = null,
                            Role = BussinessLogic.Models.Role.User
                        };
                        await _customUserManager.AddPermissionAsync(permissionDTO);                
                        taskList.Add(_customUserManager.AddPermissionAsync(permissionDTO));
                    }                   
                   //// Task.WaitAll(taskList.ToArray());
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Невозможно сохранить модель, попробуйте ещё раз");
            }

            Dropdown();
            return View(user);
        }

        private void Dropdown()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            ViewBag.Timezone = timeZones.Select(tz => new
            {
                TimezoneId = tz.Id,
                Value = tz.DisplayName
            });
        }
    }
}