using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
using Omu.ValueInjecter;

namespace Auction.Presentation.Controllers
{
    public class ProfileController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly CustomUserManager _customUserManager;
        private readonly IProductService _productService;
        private readonly ICategoriesService _categoryService;
        private readonly IBidService _bidService;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;

        public ProfileController(DataAccess.Repositories.IAuctionProvider auctionProvider, CustomUserManager customUserManager, IProductService productService, ICategoriesService categoryService, IBidService bidService)
        {
            _auctionProvider = auctionProvider;
            _customUserManager = customUserManager;
            _productService = productService;
            _categoryService = categoryService;
            _bidService = bidService;
        }

        private string ClaimId
        {
          get
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var claimId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                return claimId;
            }
        }

        public async Task<ActionResult> Index(Guid? id = null)
        {           
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var profile = await _customUserManager.FindByIdAsync(id.ToString()); 
            var profileVM = Mapper.Map<ProfileViewModel>(profile);
            profileVM.Id = Guid.Parse(profile.Id);
            return View(profileVM);
        }

        public async Task<ActionResult> Edit(Guid? id = null)
        {
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var profileDTO = await _customUserManager.FindByIdAsync(id.ToString());
            if (profileDTO == null)
            {
                return HttpNotFound();
            }

            DropdownTimeZone();
            var profileVM = Mapper.Map<ProfileViewModel>(profileDTO);
            profileVM.Id = Guid.Parse(profileDTO.Id);
            return View(profileVM);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<ActionResult> EditToPost(ProfileViewModel profileVM, HttpPostedFileBase image)
        {
            if (profileVM.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var locale = LocalizationService.GetAvalaibleLocalization();
            if ((profileVM.Locale == null) || (locale.FirstOrDefault(l => l.LocalizationId == profileVM.Locale) == null))
            {
                profileVM.Locale = LocalizationService.GetDefaultLang().LocalizationId;
            }

            if (profileVM.TimezoneId == null || profileVM.TimezoneId == string.Empty)
            {
                ModelState.AddModelError("TimezoneId", Resource.errTimeZoneNotFound);
            }

            var userDTO = Mapper.Map<Auction.BussinessLogic.Models.UserDTO>(profileVM);
            _logger.Info(string.Format("user editted profile - {0}, username -{1}, timezone - {2}", userDTO.Id, userDTO.FullName, userDTO.TimezoneId));
            if (image != null)
            {
                string extension = System.IO.Path.GetExtension(image.FileName);
                if ((extension == ".jpg") || (extension == ".png"))
                {
                    var imageBase64 = ImageHelper.LoadImage(image);
                    userDTO.Picture = imageBase64;
                }
                else
                {
                    ModelState.AddModelError("Picture", Resource.errPictureFormat);
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    await _customUserManager.EditUserAsync(userDTO);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Resource.errNotUpdate);
            }

            DropdownTimeZone();            
            return View(profileVM);
        }
      
        public ActionResult ChangePassword(Guid? id = null)
        {
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var model = new ChangePasswordModel() { Id = id.GetValueOrDefault() };

            return View(model);
        }

        [HttpPost, ActionName("ChangePassword")]
        public async Task<ActionResult> EditPassword(ChangePasswordModel model)
        {
            var user = await _customUserManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            model.OldPassword = Helpers.SecurityHelper.Hash(model.OldPassword);
            model.NewPassword = Helpers.SecurityHelper.Hash(model.NewPassword);
            model.RepeatNewPassword = Helpers.SecurityHelper.Hash(model.RepeatNewPassword);

            if (model.OldPassword != user.Password)
            {
                ModelState.AddModelError(string.Empty, Resource.errOldPassword);
            }

            if (model.OldPassword == model.NewPassword)
            {
                ModelState.AddModelError(string.Empty, Resource.errPasswordCompare);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    user.Password = model.NewPassword;
                    var userDTO = Mapper.Map<UserDTO>(user);
                    await _customUserManager.EditUserAsync(userDTO);                   
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Resource.errNotUpdate);
            }

            return View();
        }

        public async Task<ActionResult> CreateProduct(Guid? id)
        {
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            DropdownAuction();
            await DropdownCategory();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(ProductViewModel productVM, HttpPostedFileBase image, string timeTo, string auctionId)
        {
            Guid userId;
                if (ClaimId != null)
            {
                userId = new Guid(ClaimId);
            }
                else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }         

            try
            {
                var auction = config.AuctionHouses.Search(auctionId);
                if (auction == null)
                {
                    ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
                }

                Auctions.SetAuction(auction);

                DateTime dt = Convert.ToDateTime(timeTo);
                var user = await _customUserManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "user not registred");
                }
                
                dt = dt - TimeZoneHelper.ConverTimeToServer(user.TimezoneId);
                if (dt < DateTime.Now)
                {
                    ModelState.AddModelError(string.Empty, Resource.errDuration);
                }
                else
                {
                    productVM.Duration = dt - DateTime.Now;
                }

                productVM.State = State.Draft;              
                if (ModelState.IsValid)
                {
                    if (image != null)
                    {
                        string extension = System.IO.Path.GetExtension(image.FileName);
                        if ((extension == ".jpg") || (extension == ".png"))
                        {
                            var imageBase64 = ImageHelper.LoadImage(image);
                            productVM.Picture = imageBase64;
                        }
                        else
                        {
                            ModelState.AddModelError("Picture", Resource.errPictureFormat);
                        }

                        var productDTO = new ProductDTO() { Id = Guid.NewGuid() };
                        productVM.Id = productDTO.Id;
                        productVM.StartDate = DateTime.Now;
                        productVM.UserId = userId;
                        productDTO.InjectFrom(productVM);
                        await _productService.AddProductAsync(productDTO);
                        _logger.Info(string.Format("user created product on auction - {0}, username -{1}, timezone - {2}", user.Id, user.UserName, user.TimezoneId));
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("Picture", Resource.errPicture);
                    }          
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "try again");
            }

            DropdownAuction();
            await DropdownCategory();
            return View(productVM);
        }

        public async Task<ActionResult> UserProductList(Guid? id)
        {
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var productDictionary = new Dictionary<string, CatalogViewModel>();
                       
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                var productList = await _productService.ShowProductsOfUserAsync(id.GetValueOrDefault());
                var productVMList = productList.Select(pr => Mapper.Map<ProductViewModel>(pr));

                var categoryList = await _categoryService.ShowAwalaibleCategoriesAsync();
                var categoryVMList = categoryList.Select(pr => Mapper.Map<CategoryViewModel>(pr));

                if (productList.FirstOrDefault() != null)
                {
                    productDictionary.Add(auction.Name, new CatalogViewModel() { CategoriesVM = categoryVMList, ProductVM = productVMList });
                }
            }

            return View(productDictionary);
        }

        public async Task<ActionResult> UserWinProductList(Guid? id)
        {
            if (id == null)
            {
                if (ClaimId != null)
                {
                    id = new Guid(ClaimId);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }

            var catalogDictionary = new Dictionary<string, CatalogViewModel>();

            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                var bidWinnerList = await _bidService.WinnerListProductAsync(id.GetValueOrDefault());
                var productList = await _productService.ShowAwalaibleProductsWithTimeOffAsync(); 
                ////need fix to outdating
                var productWinnerList = new List<ProductDTO>();
                ProductDTO product;
                foreach (var bid in bidWinnerList)
                {
                    product = productList.FirstOrDefault(pr => pr.Id == bid.ProductId);
                    if (product != null)
                    {
                        productWinnerList.Add(product);
                    }
                }

                var productVMList = productWinnerList.Select(pr => Mapper.Map<ProductViewModel>(pr));

                var categoryList = await _categoryService.ShowAwalaibleCategoriesAsync();
                var categoryVMList = categoryList.Select(pr => Mapper.Map<CategoryViewModel>(pr));

                catalogDictionary.Add(auction.Name, new CatalogViewModel() { CategoriesVM = categoryVMList, ProductVM = productVMList });
            }

            return View(catalogDictionary);
        }

        public async Task<ActionResult> EditProduct(Guid? id, string auctionId)
        {
            DropdownAuction();
            await DropdownCategory();
            ViewBag.OldAuction = auctionId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var auction = config.AuctionHouses.Search(auctionId);
            Auctions.SetAuction(auction);

            var productDTO = await _productService.GetProductAsync(id);
            if (productDTO == null)
            {
                return HttpNotFound();
            }

            if (productDTO.State != State.Draft)
            {
                return RedirectToAction("UserProductList");
            }

            var productVM = Mapper.Map<ProductViewModel>(productDTO);

            return View(productVM);
        }

        [HttpPost, ActionName("EditProduct")]
        public async Task<ActionResult> EditToPost(ProductViewModel productVM, HttpPostedFileBase image, string auctionId, string currAuction)
        {
            if (productVM.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var oldAuction = config.AuctionHouses.Search(currAuction);
            Auctions.SetAuction(oldAuction);
            var product = await _productService.GetProductAsync(productVM.Id);

            if (product.State != State.Draft)
            {
                return RedirectToAction("UserProductList");
            }

            var productDTO = Mapper.Map<ProductDTO>(productVM);

            if (image != null)
            {
                string extension = System.IO.Path.GetExtension(image.FileName);
                if ((extension == ".jpg") || (extension == ".png"))
                {                    
                    var imageBase64 = ImageHelper.LoadImage(image);
                    productDTO.Picture = imageBase64;
                }
                else
                {
                    ModelState.AddModelError("Image", "Прикрепляемый файл должен быть *jpg/png");
                }
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (currAuction != auctionId)
                    {
                        productDTO.State = product.State;
                        productDTO.UserId = product.UserId;
                        if (productDTO.Picture == null)
                        {
                            productDTO.Picture = product.Picture;
                        }

                        await _productService.RemoveProductAsync(product.Id);
                        productDTO.StartDate = DateTime.Now;
                        var auction = config.AuctionHouses.Search(auctionId);
                        Auctions.SetAuction(auction);                       
                        await _productService.AddProductAsync(productDTO);
                    }
                    else
                    {
                        await _productService.EditProductAsync(productDTO);
                    }
                  
                   // _productService.EditProduct(productDTO);
                    return RedirectToAction("UserProductList");
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "Невозможно обновить текущее значение");
            }

            DropdownAuction();
            await DropdownCategory();
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> DeleteProduct(Guid? id, string auctionId, bool? saveChangeError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangeError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Произошла ошибка при удалении.";
            }

            var auction = config.AuctionHouses.Search(auctionId);
            Auctions.SetAuction(auction);
            ViewBag.AuctionName = auction.Name;

            var productDTO = await _productService.GetProductAsync(id);
            if (productDTO == null)
            {
                return HttpNotFound();
            }

            ViewBag.Category = (await _categoryService.ShowAwalaibleCategoriesAsync()).FirstOrDefault(c => c.Id == productDTO.CategoryID).Name;
            var productVM = Mapper.Map<ProductViewModel>(productDTO);

            return View(productVM);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProduct(Guid? id, string auctionId)
        {
            try
            {
                var auction = config.AuctionHouses.Search(auctionId);
                Auctions.SetAuction(auction);
                await _productService.RemoveProductAsync(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new { id = id, auctionId = auctionId, saveChangesError = true });
            }

            return RedirectToAction("UserProductList");
        }

        private void DropdownTimeZone()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            ViewBag.TimeZone = timeZones.Select(tz => new
            {
                timezoneId = tz.Id,
                Value = tz.DisplayName
            });
        }

        private async Task DropdownCategory()
        {
            var auction = ((IEnumerable<dynamic>)ViewBag.Auction).First();
            var currAuction = config.AuctionHouses.Search(auction.Value);

            Auctions.SetAuction(currAuction);

            var categories = await _categoryService.ShowAwalaibleCategoriesAsync();

            ViewBag.Categories = categories.Select(c => new
            {
                categoryId = c.Id,
                Value = c.Name,
            });
        }

        private void DropdownAuction()
        {
            List<AuctionViewModel> auctions = new List<AuctionViewModel>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                var type = (TypeAuctionVM)Enum.Parse(typeof(TypeAuctionVM), auction.Type);
                var auctionVM = new AuctionViewModel
                {
                    Name = auction.Name,
                    Location = auction.Location,
                    Type = type
                };
                auctions.Add(auctionVM);
            }

            ViewBag.Auction = auctions.Select(auc => new
            {
                auctionId = auc.Name,
                Value = auc.Name,
            });
        }
    }
}