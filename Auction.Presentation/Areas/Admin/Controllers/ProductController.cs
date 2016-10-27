using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Helpers;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Localization;
using Auction.Presentation.Models;
using Newtonsoft.Json;
using Omu.ValueInjecter;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class ProductController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly IProductService _productService;
        private readonly ICategoriesService _categoryService;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;       

        public ProductController(DataAccess.Repositories.IAuctionProvider auction, IProductService productService, ICategoriesService categoryService)
        {
            _auctionProvider = auction;
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<ActionResult> Index()
        {
            var productDictionary = new Dictionary<string, CatalogViewModel>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);

                var categoriesResult = await _categoryService.ShowAwalaibleCategoriesAsync();
                var categoriesList = categoriesResult.Select(m => Mapper.Map<CategoryViewModel>(m));
                var productResult = await _productService.GetProductsAsync();
                var productViewModelList = productResult.Select(c => Mapper.Map<ProductViewModel>(c));
                productDictionary.Add(auction.Name, new CatalogViewModel() { CategoriesVM = categoriesList, ProductVM = productViewModelList });
            }

            return View(productDictionary);
        }

        public async Task<ActionResult> Create()
        {
            await DropDown();
            DropdownAuction();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductViewModel productVM, HttpPostedFileBase image, string dateTo, string timeTo, string auctionId)
        {
            try
            {
                var auction = config.AuctionHouses.Search(auctionId);
                if (auction == null)
                {
                    ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
                }

                Auctions.SetAuction(auction);

                DateTime date = Convert.ToDateTime(dateTo);
                if (date <= DateTime.Now)
                {
                    ModelState.AddModelError("StartDate", Resource.errDateTime);
                }

                productVM.StartDate = date;

                DateTime duration = Convert.ToDateTime(timeTo);
                if (duration < date)
                {
                    ModelState.AddModelError("Duration", Resource.errDuration);
                }

                productVM.Duration = duration - date;

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
                        productDTO.InjectFrom(productVM);
                        await _productService.AddProductAsync(productDTO);
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

            await DropDown();
            DropdownAuction();
            return View(productVM);
        }

        public async Task<ActionResult> Edit(Guid? id, string auctionId)
        {
            await DropDown();
            DropdownAuction();
            ViewBag.OldAuction = auctionId;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            Auctions.SetAuction(auction);

            var productDTO = await _productService.GetProductAsync(id);
            if (productDTO == null)
            {
                return HttpNotFound();
            }

            var productVM = Mapper.Map<ProductViewModel>(productDTO);

            return View(productVM);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<ActionResult> EditToPost(ProductViewModel productVM, HttpPostedFileBase image, string dateTo, string timeTo, string auctionId, string currAuction)
        {
            if (productVM.Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var oldAuction = config.AuctionHouses.Search(currAuction);
            Auctions.SetAuction(oldAuction);
            var product = await _productService.GetProductAsync(productVM.Id);
            DateTime date = Convert.ToDateTime(dateTo);
            if (date < DateTime.Today)
            {
                ModelState.AddModelError("StartDate", Resource.errDateTime);
            }

            productVM.StartDate = date;

            DateTime duration = Convert.ToDateTime(timeTo);
            if (duration < date)
            {
                ModelState.AddModelError("Duration", Resource.errDuration);
            }

            productVM.Duration = duration - date;

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
                    ModelState.AddModelError("Picture", Resource.errPictureFormat);
                }
            } 
            else
            {
                ModelState.AddModelError("Picture", Resource.errPicture);
            }
                       
            try
             {
                if (ModelState.IsValid)
                {
                    if (currAuction != auctionId)
                    {
                        await _productService.RemoveProductAsync(product.Id);
                        var auction = config.AuctionHouses.Search(auctionId);
                        Auctions.SetAuction(auction);
                        await _productService.AddProductAsync(productDTO);
                    }
                    else
                    {
                        await _productService.EditProductAsync(productDTO);
                    }

                    return RedirectToAction("Index");
                }             
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Resource.errNotUpdate);
            }

            DropdownAuction();
            await DropDown();
            return View();
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

            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            Auctions.SetAuction(auction);
            ViewBag.Auction = auction;

            var productDTO = await _productService.GetProductAsync(id);
            if (productDTO == null)
            {
                return HttpNotFound();
            }

            var currCategoryResult = await _categoryService.ShowAwalaibleCategoriesAsync();
            var currCategory = currCategoryResult.FirstOrDefault(c => c.Id == productDTO.CategoryID);
            ViewBag.Category = (currCategory == null) ? null : currCategory.Name;            
            var productVM = Mapper.Map<ProductViewModel>(productDTO);

            return View(productVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid? id, string auctionId)
        {
            try
            {
                var auction = config.AuctionHouses.Search(auctionId);
                Auctions.SetAuction(auction);
                await _productService.RemoveProductAsync(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<JsonResult> GetCategory(string data)
        {
            IEnumerable<CategoryDTO> categories = null;
            if (Request.IsAjaxRequest())
            {
                dynamic auctionObj = JsonConvert.DeserializeObject(data);
                var auction = config.AuctionHouses.Search(auctionObj.auctionId.ToString());
                Auctions.SetAuction(auction);
                categories = await _categoryService.ShowAwalaibleCategoriesAsync();
                return Json(JsonConvert.SerializeObject(categories, Formatting.None));
            }

            return Json(JsonConvert.SerializeObject(categories, Formatting.None));
        }

        private async Task DropDown()
        {
            var categories = await _categoryService.ShowAwalaibleCategoriesAsync();

            ViewBag.Categories = categories.Select(c => new
            {
                categoryId = c.Id,
                Value = c.Name,
            });
        }

        private void DropdownAuction()
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

            ViewBag.Auction = auctions.Select(auc => new
            {
                auctionId = auc.Name,
                Value = auc.Name,
            });
        }
    }
}