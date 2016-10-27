using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Auction.BussinessLogic.Models;
using Auction.BussinessLogic.Services;
using Auction.Presentation.Infrastructure;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Localization;
using Auction.Presentation.Models;
using Omu.ValueInjecter;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private static AuctionHousesSection config = ConfigurationManager.GetSection("AuctionHouses") as AuctionHousesSection;
        private readonly DataAccess.Repositories.IAuctionProvider _auctionProvider;         
        private ICategoriesService _categoryService;

        public CategoryController(DataAccess.Repositories.IAuctionProvider auction, ICategoriesService categoryService)
        {
            _categoryService = categoryService;
            _auctionProvider = auction;
        }

        public async Task<ActionResult> Index()
        {
            var categoryDictionary = new Dictionary<string, IEnumerable<CategoryViewModel>>();
            foreach (AuctionHouseElement auction in config.AuctionHouses)
            {
                Auctions.SetAuction(auction);
                var category = await _categoryService.ShowAwalaibleCategoriesAsync();
                categoryDictionary.Add(auction.Name, category.Select(c => Mapper.Map<CategoryViewModel>(c)));
            }

            return View(categoryDictionary);
        }

        public ActionResult Create()
        {
            DropdownAuction();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryViewModel categoryVM, string auctionId)
        {
            if (auctionId == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            Auctions.SetAuction(auction);

            if (!await _categoryService.IsUnigueNameAsync(categoryVM.Name))
            {
                ModelState.AddModelError("Name", Resource.errNotUnique);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var categoryDTO = new CategoryDTO() { Id = Guid.NewGuid() };
                    categoryVM.Id = categoryDTO.Id;
                    categoryDTO.InjectFrom(categoryVM);
                    await _categoryService.AddCategoryAsync(categoryDTO);
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "try again");
            }

            DropdownAuction();
            return View(categoryVM);
        }

        public async Task<ActionResult> Edit(Guid? id, string auctionId)
        {
            if (id == null || auctionId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var auction = config.AuctionHouses.Search(auctionId);
            if (auction == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            Auctions.SetAuction(auction);
            DropdownAuction();

            var categoryDTO = await _categoryService.GetCategoryAsync(id);
            if (categoryDTO == null)
            {
                return HttpNotFound();
            }

            var categoryVM = Mapper.Map<CategoryViewModel>(categoryDTO);

            return View(categoryVM);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<ActionResult> EditToPost(CategoryViewModel categoryVM, string auctionId, string oldAuction)
        {
            if (categoryVM.Id == null || auctionId == null || oldAuction == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var auction = config.AuctionHouses.Search(auctionId);
            var currAuction = config.AuctionHouses.Search(oldAuction);

            if (auction == null || currAuction == null)
            {
                ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
            }

            Auctions.SetAuction(auction);
            if (!await _categoryService.IsUnigueNameAsync(categoryVM.Name))
            {
                ModelState.AddModelError("Name", Resource.errNotUnique);
            }
            else
            {
                var categoryDTO = Mapper.Map<CategoryDTO>(categoryVM);
                try
                {
                    if (currAuction.Name != auction.Name)
                    {
                        Auctions.SetAuction(currAuction);
                        await _categoryService.RemoveCategoryAsync(categoryVM.Id);
                        await _categoryService.AddCategoryAsync(categoryDTO);
                    }
                    else
                    {
                        await _categoryService.EditCategoryAsync(categoryDTO);
                    }

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Невозможно обновить текущее значение");
                }
            }

            DropdownAuction();
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

            var categoryDTO = await _categoryService.GetCategoryAsync(id);
            if (categoryDTO == null)
            {
                return HttpNotFound();
            }

            var categoryVM = Mapper.Map<CategoryViewModel>(categoryDTO);
            
            return View(categoryVM);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Guid? id, string auctionId)
        {
            try
            {
                var auction = config.AuctionHouses.Search(auctionId);
                if (auction == null)
                {
                    ModelState.AddModelError(string.Empty, Resource.errAuctionNotFound);
                }

                Auctions.SetAuction(auction);
                await _categoryService.RemoveCategoryAsync(id);
            }
            catch (Exception)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
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
                Value = auc.Name
            });
        }
    }   
}