using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Threading;
using WPF.Classes;
using WPF.Extensions;
using WPF.Models;
using WPF.Service;

namespace WPF.ViewModels
{
    [POCOWiewModel]
    public class ProductsViewModel
    {
        public ProductsViewModel()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            timer.Tick += (sender, args) =>
            {
                foreach (var item in Products)
                {
                    item.CounterSeconds -= 1;
                }
            };
            timer.Start();

            _productService = new ProductService();
            _auctionService = new AuctionService();
            _categoryService = new CategoryService();
            _bidService = new BidService();
            BidOffset = _bidService.GetBidOffset().Result;
            Auctions = new ObservableCollection<Auction>(_auctionService.GetAuctions().Result);

            SelectedAuction = DialogSelectedAuction = Auctions.FirstOrDefault();
        }

        public virtual int BidOffset { get; set; }

        public virtual int ZindexForDialog { get; set; }

        public virtual string CreateProductButtonContent { get; set; }

        public virtual string Base64ImageForNewProduct { get; set; }

        public virtual string PreviewImageFileName { get; set; }

        public virtual Product NewProduct { get; set; }

        public virtual DateTime NewProductDuration { get; set; }

        public virtual BindingList<ProductForBinding> Products { get; set; }

        public virtual ObservableCollection<Auction> Auctions { get; set; }

        public virtual ObservableCollection<Category> Categories { get; set; }

        public virtual ObservableCollection<Category> DialogCategories { get; set; }

        public virtual Auction SelectedAuction { get; set; }

        public virtual Category SelectedCategory { get; set; }

        public virtual Auction DialogSelectedAuction { get; set; }

        public virtual Category DialogSelectedCategory { get; set; }

        private IProductService _productService { get; set; }

        private IAuctionService _auctionService { get; set; }

        private ICategoryService _categoryService { get; set; }

        private IBidService _bidService { get; set; }

        public void Initialized()
        {
            ZindexForDialog = 1;
            NewProductDuration = DateTime.Now;
        }

        public void SelectedCategoryChanged()
        {
            IEnumerable<Product> result;
            IEnumerable<Bid> resultBids;
            if (SelectedCategory == null || SelectedCategory.Id == Guid.Empty)
            {
                result = _productService.GetProducts(SelectedAuction).Result;
            }            
            else
            {
                result = _productService.GetProducts(SelectedAuction, SelectedCategory.Id).Result;
            }

            resultBids = _bidService.GetBids(SelectedAuction).Result;

            Products = new BindingList<ProductForBinding>(
                result.Select(p => new ProductForBinding
                {
                    Product = p, Bid = resultBids.FirstOrDefault(b => b.ProductId == p.Id), CounterSeconds = (long)(p.StartDate + p.Duration - DateTime.Now).TotalSeconds
                }).ToList());
        }

        public void SelectedAuctionChanged()
        {
            Categories = GetCategoriesForAuction(SelectedAuction, addAll: true);
            SelectedCategory = Categories.FirstOrDefault();
        }

        public void CreateNewDialog()
        {
            ZindexForDialog = ZindexForDialog < 20 ? 20 : 0;
            NewProduct = ZindexForDialog < 20 ? null : new Product { Id = Guid.NewGuid(), StartDate = DateTime.Now, State = State.Draft };
        }

        public void DialogSelectedAuctionChanged()
        {
            DialogCategories = GetCategoriesForAuction(DialogSelectedAuction);
            DialogSelectedCategory = DialogCategories.FirstOrDefault();
        }

        public void DialogSelectedCategoryChanged()
        {
        }

        public void LoadImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Change Picture"; 
            dlg.DefaultExt = ".jpg"; 
            dlg.Filter = "Image Files (.png, .jpg)|*.png;*.jpg";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() == true)
            {
                var fileName = PreviewImageFileName = dlg.FileName;
                var image = new Bitmap(fileName);
                Base64ImageForNewProduct = image.ImageToBase64(fileName.ToUpper().Contains(".JPG") ? ImageFormat.Jpeg : ImageFormat.Png);
            }
        }

        public async void SaveNewProduct()
        {
            NewProduct.CategoryID = DialogSelectedCategory.Id;
            NewProduct.Picture = Base64ImageForNewProduct;
            NewProduct.Duration = NewProductDuration - DateTime.Now;
            await _productService.CreateProduct(NewProduct, SelectedAuction);
        }

        public void Cancel()
        {
            ZindexForDialog = 0;
        }

        public async void SendNewBid(Bid newBid)
        {
            var result = await _bidService.MakeBid(newBid, SelectedAuction);
            if (result)
            {
                var item = Products.FirstOrDefault(pr => pr.Bid.ProductId == newBid.ProductId);
                item.Bid.Price = newBid.Price;                                   
            }
        }

        protected void OnZindexForDialogChanged()
        {
            CreateProductButtonContent = ZindexForDialog < 20 ? "Create product" : "Close dialog";
        }

        private ObservableCollection<Category> GetCategoriesForAuction(Auction auction, bool addAll = false)
        {
            var tempList = new List<Category>();
            if (addAll)
            {
                tempList.Add(new Category { Id = Guid.Empty, Name = "All" });
            }

            tempList.AddRange(_categoryService.GetCategories(auction).Result);
            return new ObservableCollection<Category>(tempList);
        }
    }
}
