using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using Auction.Presentation.Areas.Admin.Models;
using Auction.Presentation.Infrastructure.Authentication;
using Auction.Presentation.Infrastructure.СustomSettings;
using Auction.Presentation.Localization;

namespace Auction.Presentation.Areas.Admin.Controllers
{
    [ClaimsAuthorize(Roles = "admin")]
    public class AuctionController : Controller
    {
        private Configuration _cfg;
        private AuctionHousesSection _section;

        public AuctionController()
        {
            _cfg = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            _section = (AuctionHousesSection)_cfg.GetSection("AuctionHouses");
        }

        public ActionResult Index()
        {
            IList<AuctionViewModel> listAuction = new List<AuctionViewModel>();
            if (_section == null)
            {
                return HttpNotFound();
            }

            foreach (AuctionHouseElement auction in _section.AuctionHouses)
            {
                var type = (TypeAuctionVM)Enum.Parse(typeof(TypeAuctionVM), auction.Type);
                listAuction.Add(new AuctionViewModel()
                {
                    Name = auction.Name,
                    Location = auction.Location,
                    Type = type
                });
            }

            return View(listAuction);
        }

        public ActionResult Create()
        {
            return View(new AuctionViewModel());
        }

        [HttpPost]
        public ActionResult Create(AuctionViewModel auction)
        {
            if (_section == null)
            {
                AuctionHousesSection newSection = new AuctionHousesSection();
                _section = newSection;            
            }           

            if (_section.AuctionHouses.Search(auction.Name) != null)
            {
                ModelState.AddModelError("Name", Resource.errNotUnique);
            }

            if (ModelState.IsValid)
            {
                _section.AuctionHouses.Add(new AuctionHouseElement()
                {
                    Name = auction.Name,
                    Location = auction.Location,
                    Type = auction.Type.ToString()
                });

                    var folder = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), auction.Location);
                    bool exists = System.IO.Directory.Exists(folder);

                    if (!exists)
                    {
                        System.IO.Directory.CreateDirectory(folder);
                    }

                _cfg.Save(ConfigurationSaveMode.Modified);
                return RedirectToAction("Index", "Auction");
            }
           
            return View();
        }

        public ActionResult Edit(string name)
        {
            if (_section == null)
            {
                return HttpNotFound();
            }

           var auction = _section.AuctionHouses.Search(name);
           if (auction == null)
            {
                return HttpNotFound();
            }

            var type = (TypeAuctionVM)Enum.Parse(typeof(TypeAuctionVM), auction.Type);
            AuctionViewModel auctionVM = new AuctionViewModel() { Name = auction.Name, Location = auction.Location, Type = type };
            
            return View(auctionVM);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditToPost(AuctionViewModel auctionVM, string oldName)
        {
            if (_section == null)
            {
                AuctionHousesSection newSection = new AuctionHousesSection();
                _section = newSection;
            }

            if (_section.AuctionHouses.Search(auctionVM.Name) != null)
            {
                ModelState.AddModelError("Name", Resource.errNotUnique);
            }

            if (ModelState.IsValid)
            {
                    var oldAuction = _section.AuctionHouses.Search(oldName);
                    var oldFolder = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), oldAuction.Location);
                    var folder = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), auctionVM.Location);
                    if (auctionVM.Type == TypeAuctionVM.Sql && auctionVM.Type.ToString().ToLower() == oldAuction.Type.ToLower())
                    {
                        if (!auctionVM.Name.Equals(oldAuction.Name))
                        {
                            System.IO.File.Move(Path.Combine(oldFolder, string.Format("{0}.{1}", oldAuction.Name, "mdf")), Path.Combine(oldFolder, string.Format("{0}.{1}", auctionVM.Name, "mdf")));
                        }
                    }

                    if (!oldFolder.Equals(folder))
                        {
                            Directory.Move(oldFolder, folder);
                        }
                    
                _section.AuctionHouses.Remove(oldName);
                _section.AuctionHouses.Add(new AuctionHouseElement()
                {
                    Name = auctionVM.Name,
                    Location = auctionVM.Location,
                    Type = auctionVM.Type.ToString()
                });

                _cfg.Save(ConfigurationSaveMode.Modified);
                return RedirectToAction("Index");
            }

            return View();   
        }

       public ActionResult Delete(string name)
        {
            if (_section == null)
            {
                return HttpNotFound();
            }

            var auction = _section.AuctionHouses.Search(name);
            if (auction == null)
            {
                return HttpNotFound();
            }

            var type = (TypeAuctionVM)Enum.Parse(typeof(TypeAuctionVM), auction.Type);
            AuctionViewModel auctionVM = new AuctionViewModel()
            {
                Name = auction.Name,
                Location = auction.Location,
                Type = type
            };

            return View(auctionVM);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult RemoveToPost(string name)
        {
            if (_section == null)
            {
                return HttpNotFound();
            }

            var auction = _section.AuctionHouses.Search(name);

            if (auction.Type == TypeAuctionVM.Json.ToString())
            {
                var folder = Path.Combine(HostingEnvironment.MapPath("~/App_Data/"), auction.Location);

                bool exists = System.IO.Directory.Exists(folder);
                if (exists)
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(folder);

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }

                    System.IO.Directory.Delete(folder, true);
                }
            }

             _section.AuctionHouses.Remove(name);
            _cfg.Save(ConfigurationSaveMode.Modified);
            return RedirectToAction("Index", "Auction");
        }      
    }
}